defmodule LightwarriorWeb.IPlayerLiveTest do
  use LightwarriorWeb.ConnCase

  import Phoenix.LiveViewTest
  import Lightwarrior.ImageplayerFixtures

  @create_attrs %{}
  @update_attrs %{}
  @invalid_attrs %{}

  defp create_i_player(_) do
    i_player = i_player_fixture()
    %{i_player: i_player}
  end

  describe "Index" do
    setup [:create_i_player]

    test "lists all iplayer", %{conn: conn} do
      {:ok, _index_live, html} = live(conn, ~p"/iplayer")

      assert html =~ "Listing Iplayer"
    end

    test "saves new i_player", %{conn: conn} do
      {:ok, index_live, _html} = live(conn, ~p"/iplayer")

      assert index_live |> element("a", "New I player") |> render_click() =~
               "New I player"

      assert_patch(index_live, ~p"/iplayer/new")

      assert index_live
             |> form("#i_player-form", i_player: @invalid_attrs)
             |> render_change() =~ "can&#39;t be blank"

      assert index_live
             |> form("#i_player-form", i_player: @create_attrs)
             |> render_submit()

      assert_patch(index_live, ~p"/iplayer")

      html = render(index_live)
      assert html =~ "I player created successfully"
    end

    test "updates i_player in listing", %{conn: conn, i_player: i_player} do
      {:ok, index_live, _html} = live(conn, ~p"/iplayer")

      assert index_live |> element("#iplayer-#{i_player.id} a", "Edit") |> render_click() =~
               "Edit I player"

      assert_patch(index_live, ~p"/iplayer/#{i_player}/edit")

      assert index_live
             |> form("#i_player-form", i_player: @invalid_attrs)
             |> render_change() =~ "can&#39;t be blank"

      assert index_live
             |> form("#i_player-form", i_player: @update_attrs)
             |> render_submit()

      assert_patch(index_live, ~p"/iplayer")

      html = render(index_live)
      assert html =~ "I player updated successfully"
    end

    test "deletes i_player in listing", %{conn: conn, i_player: i_player} do
      {:ok, index_live, _html} = live(conn, ~p"/iplayer")

      assert index_live |> element("#iplayer-#{i_player.id} a", "Delete") |> render_click()
      refute has_element?(index_live, "#iplayer-#{i_player.id}")
    end
  end

  describe "Show" do
    setup [:create_i_player]

    test "displays i_player", %{conn: conn, i_player: i_player} do
      {:ok, _show_live, html} = live(conn, ~p"/iplayer/#{i_player}")

      assert html =~ "Show I player"
    end

    test "updates i_player within modal", %{conn: conn, i_player: i_player} do
      {:ok, show_live, _html} = live(conn, ~p"/iplayer/#{i_player}")

      assert show_live |> element("a", "Edit") |> render_click() =~
               "Edit I player"

      assert_patch(show_live, ~p"/iplayer/#{i_player}/show/edit")

      assert show_live
             |> form("#i_player-form", i_player: @invalid_attrs)
             |> render_change() =~ "can&#39;t be blank"

      assert show_live
             |> form("#i_player-form", i_player: @update_attrs)
             |> render_submit()

      assert_patch(show_live, ~p"/iplayer/#{i_player}")

      html = render(show_live)
      assert html =~ "I player updated successfully"
    end
  end
end
