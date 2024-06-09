defmodule Lightwarrior.HyperionFixtures do
  @moduledoc """
  This module defines test helpers for creating
  entities via the `Lightwarrior.Hyperion` context.
  """

  @doc """
  Generate a hyperion_led_mapping.
  """
  def hyperion_led_mapping_fixture(attrs \\ %{}) do
    {:ok, hyperion_led_mapping} =
      attrs
      |> Enum.into(%{

      })
      |> Lightwarrior.Hyperion.create_hyperion_led_mapping()

    hyperion_led_mapping
  end
end
