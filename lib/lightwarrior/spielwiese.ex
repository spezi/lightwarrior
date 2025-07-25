defmodule Lightwarrior.Spielwiese do
  @moduledoc """
  The Spielwiese context.
  """

  #import Ecto.Query, warn: false
  #alias Lightwarrior.Repo

  #alias Lightwarrior.Spielwiese.Test

  @doc """
  Returns the list of tests.

  ## Examples

      iex> list_tests()
      [%Test{}, ...]

  """
  def list_tests do
    raise "TODO"
  end

  @doc """
  Gets a single test.

  Raises if the Test does not exist.

  ## Examples

      iex> get_test!(123)
      %Test{}

  """
  def get_test!(_id), do: raise "TODO"

  @doc """
  Creates a test.

  ## Examples

      iex> create_test(%{field: value})
      {:ok, %Test{}}

      iex> create_test(%{field: bad_value})
      {:error, ...}

  """
  def create_test(_attrs) do
    raise "TODO"
  end

  @doc """
  Updates a test.

  ## Examples

      iex> update_test(test, %{field: new_value})
      {:ok, %Test{}}

      iex> update_test(test, %{field: bad_value})
      {:error, ...}

  """
  def update_test(%{} = _test, _attrs) do
    raise "TODO"
  end

  @doc """
  Deletes a Test.

  ## Examples

      iex> delete_test(test)
      {:ok, %Test{}}

      iex> delete_test(test)
      {:error, ...}

  """
  def delete_test(%{} = _test) do
    raise "TODO"
  end

  @doc """
  Returns a data structure for tracking test changes.

  ## Examples

      iex> change_test(test)
      %Todo{...}

  """
  def change_test(%{} = _test, _attrs \\ %{}) do
    raise "TODO"
  end
end
