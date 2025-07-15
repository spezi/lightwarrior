defmodule LightwarriorWeb.HyperionConfigLiveTest do
  use LightwarriorWeb.ConnCase

  import Phoenix.LiveViewTest
  import Lightwarrior.HyperionFixtures

  @create_attrs %{name: "some name"}
  @update_attrs %{name: "some updated name"}
  @invalid_attrs %{name: nil}
  defp create_hyperion_config(_) do
    hyperion_config = hyperion_config_fixture()

    %{hyperion_config: hyperion_config}
  end

  describe "Index" do
    setup [:create_hyperion_config]

    test "lists all hyperionconfigs", %{conn: conn, hyperion_config: hyperion_config} do
      {:ok, _index_live, html} = live(conn, ~p"/hyperionconfigs")

      assert html =~ "Listing Hyperionconfigs"
      assert html =~ hyperion_config.name
    end

    test "saves new hyperion_config", %{conn: conn} do
      {:ok, index_live, _html} = live(conn, ~p"/hyperionconfigs")

      assert {:ok, form_live, _} =
               index_live
               |> element("a", "New Hyperion config")
               |> render_click()
               |> follow_redirect(conn, ~p"/hyperionconfigs/new")

      assert render(form_live) =~ "New Hyperion config"

      assert form_live
             |> form("#hyperion_config-form", hyperion_config: @invalid_attrs)
             |> render_change() =~ "can&#39;t be blank"

      assert {:ok, index_live, _html} =
               form_live
               |> form("#hyperion_config-form", hyperion_config: @create_attrs)
               |> render_submit()
               |> follow_redirect(conn, ~p"/hyperionconfigs")

      html = render(index_live)
      assert html =~ "Hyperion config created successfully"
      assert html =~ "some name"
    end

    test "updates hyperion_config in listing", %{conn: conn, hyperion_config: hyperion_config} do
      {:ok, index_live, _html} = live(conn, ~p"/hyperionconfigs")

      assert {:ok, form_live, _html} =
               index_live
               |> element("#hyperionconfigs-#{hyperion_config.id} a", "Edit")
               |> render_click()
               |> follow_redirect(conn, ~p"/hyperionconfigs/#{hyperion_config}/edit")

      assert render(form_live) =~ "Edit Hyperion config"

      assert form_live
             |> form("#hyperion_config-form", hyperion_config: @invalid_attrs)
             |> render_change() =~ "can&#39;t be blank"

      assert {:ok, index_live, _html} =
               form_live
               |> form("#hyperion_config-form", hyperion_config: @update_attrs)
               |> render_submit()
               |> follow_redirect(conn, ~p"/hyperionconfigs")

      html = render(index_live)
      assert html =~ "Hyperion config updated successfully"
      assert html =~ "some updated name"
    end

    test "deletes hyperion_config in listing", %{conn: conn, hyperion_config: hyperion_config} do
      {:ok, index_live, _html} = live(conn, ~p"/hyperionconfigs")

      assert index_live |> element("#hyperionconfigs-#{hyperion_config.id} a", "Delete") |> render_click()
      refute has_element?(index_live, "#hyperionconfigs-#{hyperion_config.id}")
    end
  end

  describe "Show" do
    setup [:create_hyperion_config]

    test "displays hyperion_config", %{conn: conn, hyperion_config: hyperion_config} do
      {:ok, _show_live, html} = live(conn, ~p"/hyperionconfigs/#{hyperion_config}")

      assert html =~ "Show Hyperion config"
      assert html =~ hyperion_config.name
    end

    test "updates hyperion_config and returns to show", %{conn: conn, hyperion_config: hyperion_config} do
      {:ok, show_live, _html} = live(conn, ~p"/hyperionconfigs/#{hyperion_config}")

      assert {:ok, form_live, _} =
               show_live
               |> element("a", "Edit")
               |> render_click()
               |> follow_redirect(conn, ~p"/hyperionconfigs/#{hyperion_config}/edit?return_to=show")

      assert render(form_live) =~ "Edit Hyperion config"

      assert form_live
             |> form("#hyperion_config-form", hyperion_config: @invalid_attrs)
             |> render_change() =~ "can&#39;t be blank"

      assert {:ok, show_live, _html} =
               form_live
               |> form("#hyperion_config-form", hyperion_config: @update_attrs)
               |> render_submit()
               |> follow_redirect(conn, ~p"/hyperionconfigs/#{hyperion_config}")

      html = render(show_live)
      assert html =~ "Hyperion config updated successfully"
      assert html =~ "some updated name"
    end
  end
end
