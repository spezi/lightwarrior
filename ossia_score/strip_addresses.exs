#!/usr/bin/env elixir

# Standalone ossia score address stripper.
#
#   elixir strip_addresses.exs <score-file> [prefix] [--overwrite]
#
# Examples:
#   elixir strip_addresses.exs triebwerke.score
#   elixir strip_addresses.exs triebwerke.score grandma
#   elixir strip_addresses.exs triebwerke.score output --overwrite
#
# Without --overwrite the result is written next to the input as
# "<name>.stripped.score".

Mix.install([{:jason, "~> 1.4"}])

defmodule OssiaScore.AddressStripper do
  @default_prefix "grandma"

  def strip_file(path, prefix \\ @default_prefix) when is_binary(path) and is_binary(prefix) do
    with {:ok, raw} <- File.read(path),
         {:ok, data} <- Jason.decode(raw) do
      data
      |> strip_addresses(prefix)
      |> Jason.encode()
    end
  end

  def strip_file_to(path, prefix \\ @default_prefix, opts \\ [])
      when is_binary(path) and is_binary(prefix) do
    out = resolve_out(path, Keyword.get(opts, :out))

    with {:ok, json} <- strip_file(path, prefix),
         :ok <- File.write(out, json) do
      {:ok, out}
    end
  end

  def strip_addresses(tree, prefix \\ @default_prefix) when is_binary(prefix) do
    re = Regex.compile!("^" <> Regex.escape(prefix) <> ":/")
    do_strip(tree, re)
  end

  defp do_strip(%{} = map, re) do
    Map.new(map, fn
      {"Address", value} -> {"Address", rewrite(value, re)}
      {key, value} -> {key, do_strip(value, re)}
    end)
  end

  defp do_strip(list, re) when is_list(list), do: Enum.map(list, &do_strip(&1, re))
  defp do_strip(other, _re), do: other

  defp rewrite(value, re) when is_binary(value), do: Regex.replace(re, value, ":/")
  defp rewrite(value, re), do: do_strip(value, re)

  defp resolve_out(path, nil), do: Path.rootname(path) <> ".stripped" <> Path.extname(path)
  defp resolve_out(path, :overwrite), do: path
  defp resolve_out(_path, out) when is_binary(out), do: out
end

# --- tiny CLI front-end ---------------------------------------------------

{flags, positional, _} =
  OptionParser.parse(System.argv(), switches: [overwrite: :boolean])

case positional do
  [] ->
    IO.puts(:stderr, "usage: elixir strip_addresses.exs <score-file> [prefix] [--overwrite]")
    System.halt(1)

  [path | rest] ->
    prefix = List.first(rest) || "grandma"
    opts = if flags[:overwrite], do: [out: :overwrite], else: []

    case OssiaScore.AddressStripper.strip_file_to(path, prefix, opts) do
      {:ok, out} ->
        IO.puts("Stripped '#{prefix}:' addresses -> #{out}")

      {:error, reason} ->
        IO.puts(:stderr, "error: #{inspect(reason)}")
        System.halt(1)
    end
end
