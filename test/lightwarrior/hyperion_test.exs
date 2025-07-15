defmodule Lightwarrior.HyperionTest do
  use Lightwarrior.DataCase

  alias Lightwarrior.Hyperion

  describe "hyperionconfigs" do
    alias Lightwarrior.Hyperion.HyperionConfig

    import Lightwarrior.HyperionFixtures

    @invalid_attrs %{name: nil}

    test "list_hyperionconfigs/0 returns all hyperionconfigs" do
      hyperion_config = hyperion_config_fixture()
      assert Hyperion.list_hyperionconfigs() == [hyperion_config]
    end

    test "get_hyperion_config!/1 returns the hyperion_config with given id" do
      hyperion_config = hyperion_config_fixture()
      assert Hyperion.get_hyperion_config!(hyperion_config.id) == hyperion_config
    end

    test "create_hyperion_config/1 with valid data creates a hyperion_config" do
      valid_attrs = %{name: "some name"}

      assert {:ok, %HyperionConfig{} = hyperion_config} = Hyperion.create_hyperion_config(valid_attrs)
      assert hyperion_config.name == "some name"
    end

    test "create_hyperion_config/1 with invalid data returns error changeset" do
      assert {:error, %Ecto.Changeset{}} = Hyperion.create_hyperion_config(@invalid_attrs)
    end

    test "update_hyperion_config/2 with valid data updates the hyperion_config" do
      hyperion_config = hyperion_config_fixture()
      update_attrs = %{name: "some updated name"}

      assert {:ok, %HyperionConfig{} = hyperion_config} = Hyperion.update_hyperion_config(hyperion_config, update_attrs)
      assert hyperion_config.name == "some updated name"
    end

    test "update_hyperion_config/2 with invalid data returns error changeset" do
      hyperion_config = hyperion_config_fixture()
      assert {:error, %Ecto.Changeset{}} = Hyperion.update_hyperion_config(hyperion_config, @invalid_attrs)
      assert hyperion_config == Hyperion.get_hyperion_config!(hyperion_config.id)
    end

    test "delete_hyperion_config/1 deletes the hyperion_config" do
      hyperion_config = hyperion_config_fixture()
      assert {:ok, %HyperionConfig{}} = Hyperion.delete_hyperion_config(hyperion_config)
      assert_raise Ecto.NoResultsError, fn -> Hyperion.get_hyperion_config!(hyperion_config.id) end
    end

    test "change_hyperion_config/1 returns a hyperion_config changeset" do
      hyperion_config = hyperion_config_fixture()
      assert %Ecto.Changeset{} = Hyperion.change_hyperion_config(hyperion_config)
    end
  end
end
