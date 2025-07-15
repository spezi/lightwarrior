defmodule Lightwarrior.SpielwieseFixtures do
  @moduledoc """
  This module defines test helpers for creating
  entities via the `Lightwarrior.Spielwiese` context.
  """

  @doc """
  Generate a test.
  """
  def test_fixture(attrs \\ %{}) do
    {:ok, test} =
      attrs
      |> Enum.into(%{
        name: "some name"
      })
      |> Lightwarrior.Spielwiese.create_test()

    test
  end
end
