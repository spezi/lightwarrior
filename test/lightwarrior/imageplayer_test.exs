defmodule Lightwarrior.ImageplayerTest do
  use Lightwarrior.DataCase

  alias Lightwarrior.Imageplayer

  describe "iplayer" do
    alias Lightwarrior.Imageplayer.IPlayer

    import Lightwarrior.ImageplayerFixtures

    @invalid_attrs %{}

    test "list_iplayer/0 returns all iplayer" do
      i_player = i_player_fixture()
      assert Imageplayer.list_iplayer() == [i_player]
    end

    test "get_i_player!/1 returns the i_player with given id" do
      i_player = i_player_fixture()
      assert Imageplayer.get_i_player!(i_player.id) == i_player
    end

    test "create_i_player/1 with valid data creates a i_player" do
      valid_attrs = %{}

      assert {:ok, %IPlayer{} = i_player} = Imageplayer.create_i_player(valid_attrs)
    end

    test "create_i_player/1 with invalid data returns error changeset" do
      assert {:error, %Ecto.Changeset{}} = Imageplayer.create_i_player(@invalid_attrs)
    end

    test "update_i_player/2 with valid data updates the i_player" do
      i_player = i_player_fixture()
      update_attrs = %{}

      assert {:ok, %IPlayer{} = i_player} = Imageplayer.update_i_player(i_player, update_attrs)
    end

    test "update_i_player/2 with invalid data returns error changeset" do
      i_player = i_player_fixture()
      assert {:error, %Ecto.Changeset{}} = Imageplayer.update_i_player(i_player, @invalid_attrs)
      assert i_player == Imageplayer.get_i_player!(i_player.id)
    end

    test "delete_i_player/1 deletes the i_player" do
      i_player = i_player_fixture()
      assert {:ok, %IPlayer{}} = Imageplayer.delete_i_player(i_player)
      assert_raise Ecto.NoResultsError, fn -> Imageplayer.get_i_player!(i_player.id) end
    end

    test "change_i_player/1 returns a i_player changeset" do
      i_player = i_player_fixture()
      assert %Ecto.Changeset{} = Imageplayer.change_i_player(i_player)
    end
  end
end
