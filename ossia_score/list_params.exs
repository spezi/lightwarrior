#!/usr/bin/env elixir

# Lists all parameters in an ossia score whose Address uses a given prefix,
# together with the node (process) each parameter belongs to.
#
#   elixir list_params.exs <score-file> [prefix] [--csv]
#
# Examples:
#   elixir list_params.exs triebwerke.score
#   elixir list_params.exs triebwerke.score grandma
#   elixir list_params.exs triebwerke.score grandma --csv > params.csv
#
# Prefix defaults to "grandma". Without --csv a readable table is printed,
# sorted by address (natural channel order).

Mix.install([{:jason, "~> 1.4"}])

defmodule OssiaScore.ParamLister do
  @default_prefix "grandma"

  @doc "Returns {:ok, [param_map]} | {:error, reason}."
  def list_file(path, prefix \\ @default_prefix) when is_binary(path) and is_binary(prefix) do
    with {:ok, raw} <- File.read(path),
         {:ok, data} <- Jason.decode(raw) do
      {:ok, list_params(data, prefix)}
    end
  end

  @doc "Walks a decoded score, returns the matching parameters as maps."
  def list_params(tree, prefix \\ @default_prefix) when is_binary(prefix) do
    re = Regex.compile!("^" <> Regex.escape(prefix) <> ":/")

    tree
    |> collect(re, nil, [])
    |> Enum.reverse()
    |> Enum.sort_by(&sort_key(&1.address))
  end

  # Depth-first walk. `node` tracks the nearest enclosing object that carries
  # a "Metadata" block (i.e. the process/node the parameter lives in).
  defp collect(%{} = map, re, node, acc) do
    node = if metadata?(map), do: map, else: node

    acc =
      case Map.get(map, "Address") do
        addr when is_binary(addr) ->
          if Regex.match?(re, addr), do: [to_param(map, addr, node) | acc], else: acc

        _ ->
          acc
      end

    Enum.reduce(map, acc, fn {_k, v}, a -> collect(v, re, node, a) end)
  end

  defp collect(list, re, node, acc) when is_list(list),
    do: Enum.reduce(list, acc, fn v, a -> collect(v, re, node, a) end)

  defp collect(_other, _re, _node, acc), do: acc

  defp metadata?(%{"Metadata" => %{}}), do: true
  defp metadata?(_), do: false

  defp to_param(obj, addr, node) do
    %{
      name: Map.get(obj, "Custom", ""),
      node: node_name(node),
      address: addr,
      domain: obj |> Map.get("Domain") |> format_domain()
    }
  end

  # The human-readable node name shown in the GUI lives in Metadata.ScriptingName.
  defp node_name(%{"Metadata" => md} = obj) do
    cond do
      is_binary(md["ScriptingName"]) and md["ScriptingName"] != "" -> md["ScriptingName"]
      is_binary(md["Label"]) and md["Label"] != "" -> md["Label"]
      true -> Map.get(obj, "ObjectName", "")
    end
  end

  defp node_name(_), do: ""

  defp format_domain(%{"Float" => %{"Min" => mn, "Max" => mx}}),
    do: "#{trim_float(mn)}..#{trim_float(mx)}"

  defp format_domain(%{"Int" => %{"Min" => mn, "Max" => mx}}), do: "#{mn}..#{mx}"
  defp format_domain(_), do: ""

  defp trim_float(f) when is_float(f) do
    s = :erlang.float_to_binary(f, [:short])
    String.replace_suffix(s, ".0", "")
  end

  defp trim_float(other), do: to_string(other)

  # Natural sort so /ch/2 comes before /ch/10.
  defp sort_key(addr) do
    addr
    |> String.split(~r/(\d+)/, include_captures: true, trim: true)
    |> Enum.map(fn part ->
      case Integer.parse(part) do
        {n, ""} -> {1, n, ""}
        _ -> {0, 0, part}
      end
    end)
  end
end

# --- CLI ------------------------------------------------------------------

defmodule CLI do
  def main(argv) do
    {flags, positional, _} = OptionParser.parse(argv, switches: [csv: :boolean])

    case positional do
      [] ->
        IO.puts(:stderr, "usage: elixir list_params.exs <score-file> [prefix] [--csv]")
        System.halt(1)

      [path | rest] ->
        prefix = List.first(rest) || "grandma"

        case OssiaScore.ParamLister.list_file(path, prefix) do
          {:ok, params} -> render(params, prefix, flags[:csv])
          {:error, reason} -> die(reason)
        end
    end
  end

  defp render([], prefix, _csv) do
    IO.puts("No parameters found with prefix '#{prefix}:'.")
  end

  defp render(params, _prefix, true) do
    IO.puts("parameter,node,address,domain")

    Enum.each(params, fn p ->
      IO.puts([csv(p.name), ",", csv(p.node), ",", csv(p.address), ",", csv(p.domain)])
    end)
  end

  defp render(params, prefix, _csv) do
    name_w = width(params, & &1.name, "PARAMETER")
    node_w = width(params, & &1.node, "NODE")
    addr_w = width(params, & &1.address, "ADDRESS")

    IO.puts(row("PARAMETER", name_w, "NODE", node_w, "ADDRESS", addr_w, "DOMAIN"))
    IO.puts(row(dash(name_w), name_w, dash(node_w), node_w, dash(addr_w), addr_w, dash(6)))

    Enum.each(params, fn p ->
      IO.puts(row(p.name, name_w, p.node, node_w, p.address, addr_w, p.domain))
    end)

    IO.puts("\n#{length(params)} parameter(s) with prefix '#{prefix}:'.")
  end

  defp row(a, aw, b, bw, c, cw, d) do
    [pad(a, aw), "  ", pad(b, bw), "  ", pad(c, cw), "  ", d]
  end

  defp width(params, fun, header) do
    params |> Enum.map(&String.length(fun.(&1))) |> Enum.max() |> max(String.length(header))
  end

  defp pad(s, w), do: String.pad_trailing(s, w)
  defp dash(w), do: String.duplicate("-", w)

  defp csv(s) do
    if String.contains?(s, [",", "\"", "\n"]),
      do: "\"" <> String.replace(s, "\"", "\"\"") <> "\"",
      else: s
  end

  defp die(reason) do
    IO.puts(:stderr, "error: #{inspect(reason)}")
    System.halt(1)
  end
end

CLI.main(System.argv())
