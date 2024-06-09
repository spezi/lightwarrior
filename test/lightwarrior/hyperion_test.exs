defmodule Lightwarrior.HyperionTest do
  use Lightwarrior.DataCase

  alias Lightwarrior.Hyperion

  describe "hyperionledmappings" do
    alias Lightwarrior.Hyperion.HyperionLEDMapping

    import Lightwarrior.HyperionFixtures

    @invalid_attrs %{}

    test "list_hyperionledmappings/0 returns all hyperionledmappings" do
      hyperion_led_mapping = hyperion_led_mapping_fixture()
      assert Hyperion.list_hyperionledmappings() == [hyperion_led_mapping]
    end

    test "get_hyperion_led_mapping!/1 returns the hyperion_led_mapping with given id" do
      hyperion_led_mapping = hyperion_led_mapping_fixture()
      assert Hyperion.get_hyperion_led_mapping!(hyperion_led_mapping.id) == hyperion_led_mapping
    end

    test "create_hyperion_led_mapping/1 with valid data creates a hyperion_led_mapping" do
      valid_attrs = %{}

      assert {:ok, %HyperionLEDMapping{} = hyperion_led_mapping} = Hyperion.create_hyperion_led_mapping(valid_attrs)
    end

    test "create_hyperion_led_mapping/1 with invalid data returns error changeset" do
      assert {:error, %Ecto.Changeset{}} = Hyperion.create_hyperion_led_mapping(@invalid_attrs)
    end

    test "update_hyperion_led_mapping/2 with valid data updates the hyperion_led_mapping" do
      hyperion_led_mapping = hyperion_led_mapping_fixture()
      update_attrs = %{}

      assert {:ok, %HyperionLEDMapping{} = hyperion_led_mapping} = Hyperion.update_hyperion_led_mapping(hyperion_led_mapping, update_attrs)
    end

    test "update_hyperion_led_mapping/2 with invalid data returns error changeset" do
      hyperion_led_mapping = hyperion_led_mapping_fixture()
      assert {:error, %Ecto.Changeset{}} = Hyperion.update_hyperion_led_mapping(hyperion_led_mapping, @invalid_attrs)
      assert hyperion_led_mapping == Hyperion.get_hyperion_led_mapping!(hyperion_led_mapping.id)
    end

    test "delete_hyperion_led_mapping/1 deletes the hyperion_led_mapping" do
      hyperion_led_mapping = hyperion_led_mapping_fixture()
      assert {:ok, %HyperionLEDMapping{}} = Hyperion.delete_hyperion_led_mapping(hyperion_led_mapping)
      assert_raise Ecto.NoResultsError, fn -> Hyperion.get_hyperion_led_mapping!(hyperion_led_mapping.id) end
    end

    test "change_hyperion_led_mapping/1 returns a hyperion_led_mapping changeset" do
      hyperion_led_mapping = hyperion_led_mapping_fixture()
      assert %Ecto.Changeset{} = Hyperion.change_hyperion_led_mapping(hyperion_led_mapping)
    end
  end
end
