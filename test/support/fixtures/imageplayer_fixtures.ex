defmodule Lightwarrior.ImageplayerFixtures do
  @moduledoc """
  This module defines test helpers for creating
  entities via the `Lightwarrior.Imageplayer` context.
  """

  @doc """
  Generate a i_player.
  """
  def i_player_fixture(attrs \\ %{}) do
    {:ok, i_player} =
      attrs
      |> Enum.into(%{

      })
      |> Lightwarrior.Imageplayer.create_i_player()

    i_player
  end
end
