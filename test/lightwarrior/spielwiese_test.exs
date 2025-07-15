defmodule Lightwarrior.SpielwieseTest do
  use Lightwarrior.DataCase

  alias Lightwarrior.Spielwiese

  describe "tests" do
    alias Lightwarrior.Spielwiese.Test

    import Lightwarrior.SpielwieseFixtures

    @invalid_attrs %{name: nil}

    test "list_tests/0 returns all tests" do
      test = test_fixture()
      assert Spielwiese.list_tests() == [test]
    end

    test "get_test!/1 returns the test with given id" do
      test = test_fixture()
      assert Spielwiese.get_test!(test.id) == test
    end

    test "create_test/1 with valid data creates a test" do
      valid_attrs = %{name: "some name"}

      assert {:ok, %Test{} = test} = Spielwiese.create_test(valid_attrs)
      assert test.name == "some name"
    end

    test "create_test/1 with invalid data returns error changeset" do
      assert {:error, %Ecto.Changeset{}} = Spielwiese.create_test(@invalid_attrs)
    end

    test "update_test/2 with valid data updates the test" do
      test = test_fixture()
      update_attrs = %{name: "some updated name"}

      assert {:ok, %Test{} = test} = Spielwiese.update_test(test, update_attrs)
      assert test.name == "some updated name"
    end

    test "update_test/2 with invalid data returns error changeset" do
      test = test_fixture()
      assert {:error, %Ecto.Changeset{}} = Spielwiese.update_test(test, @invalid_attrs)
      assert test == Spielwiese.get_test!(test.id)
    end

    test "delete_test/1 deletes the test" do
      test = test_fixture()
      assert {:ok, %Test{}} = Spielwiese.delete_test(test)
      assert_raise Ecto.NoResultsError, fn -> Spielwiese.get_test!(test.id) end
    end

    test "change_test/1 returns a test changeset" do
      test = test_fixture()
      assert %Ecto.Changeset{} = Spielwiese.change_test(test)
    end
  end
end
