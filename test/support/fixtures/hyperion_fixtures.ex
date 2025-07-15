defmodule Lightwarrior.HyperionFixtures do
  @moduledoc """
  This module defines test helpers for creating
  entities via the `Lightwarrior.Hyperion` context.
  """

  @doc """
  Generate a hyperion_config.
  """
  def hyperion_config_fixture(attrs \\ %{}) do
    {:ok, hyperion_config} =
      attrs
      |> Enum.into(%{
        name: "some name"
      })
      |> Lightwarrior.Hyperion.create_hyperion_config()

    hyperion_config
  end
end
