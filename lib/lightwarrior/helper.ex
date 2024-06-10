defmodule Lightwarrior.Helper do
  @moduledoc """
  litle helper functions
  """

  def string_keys_to_atom_keys(map) when is_map(map) do
    map
    |> Enum.reduce(%{}, fn {key, value}, acc ->
      atom_key = String.to_existing_atom(key)
      new_value =
        if is_map(value) do
          string_keys_to_atom_keys(value)
        else
          value
        end
      Map.put(acc, atom_key, new_value)
    end)
  end

end
