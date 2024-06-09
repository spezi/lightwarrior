defmodule LightwarriorWeb.HyperionLEDMappingLiveTest do
  use LightwarriorWeb.ConnCase

  import Phoenix.LiveViewTest
  import Lightwarrior.HyperionFixtures

  @create_attrs %{}
  @update_attrs %{}
  @invalid_attrs %{}

  defp create_hyperion_led_mapping(_) do
    hyperion_led_mapping = hyperion_led_mapping_fixture()
    %{hyperion_led_mapping: hyperion_led_mapping}
  end

  describe "Index" do
    setup [:create_hyperion_led_mapping]

    test "lists all hyperionledmappings", %{conn: conn} do
      {:ok, _index_live, html} = live(conn, ~p"/hyperionledmappings")

      assert html =~ "Listing Hyperionledmappings"
    end

    test "saves new hyperion_led_mapping", %{conn: conn} do
      {:ok, index_live, _html} = live(conn, ~p"/hyperionledmappings")

      assert index_live |> element("a", "New Hyperion led mapping") |> render_click() =~
               "New Hyperion led mapping"

      assert_patch(index_live, ~p"/hyperionledmappings/new")

      assert index_live
             |> form("#hyperion_led_mapping-form", hyperion_led_mapping: @invalid_attrs)
             |> render_change() =~ "can&#39;t be blank"

      assert index_live
             |> form("#hyperion_led_mapping-form", hyperion_led_mapping: @create_attrs)
             |> render_submit()

      assert_patch(index_live, ~p"/hyperionledmappings")

      html = render(index_live)
      assert html =~ "Hyperion led mapping created successfully"
    end

    test "updates hyperion_led_mapping in listing", %{conn: conn, hyperion_led_mapping: hyperion_led_mapping} do
      {:ok, index_live, _html} = live(conn, ~p"/hyperionledmappings")

      assert index_live |> element("#hyperionledmappings-#{hyperion_led_mapping.id} a", "Edit") |> render_click() =~
               "Edit Hyperion led mapping"

      assert_patch(index_live, ~p"/hyperionledmappings/#{hyperion_led_mapping}/edit")

      assert index_live
             |> form("#hyperion_led_mapping-form", hyperion_led_mapping: @invalid_attrs)
             |> render_change() =~ "can&#39;t be blank"

      assert index_live
             |> form("#hyperion_led_mapping-form", hyperion_led_mapping: @update_attrs)
             |> render_submit()

      assert_patch(index_live, ~p"/hyperionledmappings")

      html = render(index_live)
      assert html =~ "Hyperion led mapping updated successfully"
    end

    test "deletes hyperion_led_mapping in listing", %{conn: conn, hyperion_led_mapping: hyperion_led_mapping} do
      {:ok, index_live, _html} = live(conn, ~p"/hyperionledmappings")

      assert index_live |> element("#hyperionledmappings-#{hyperion_led_mapping.id} a", "Delete") |> render_click()
      refute has_element?(index_live, "#hyperionledmappings-#{hyperion_led_mapping.id}")
    end
  end

  describe "Show" do
    setup [:create_hyperion_led_mapping]

    test "displays hyperion_led_mapping", %{conn: conn, hyperion_led_mapping: hyperion_led_mapping} do
      {:ok, _show_live, html} = live(conn, ~p"/hyperionledmappings/#{hyperion_led_mapping}")

      assert html =~ "Show Hyperion led mapping"
    end

    test "updates hyperion_led_mapping within modal", %{conn: conn, hyperion_led_mapping: hyperion_led_mapping} do
      {:ok, show_live, _html} = live(conn, ~p"/hyperionledmappings/#{hyperion_led_mapping}")

      assert show_live |> element("a", "Edit") |> render_click() =~
               "Edit Hyperion led mapping"

      assert_patch(show_live, ~p"/hyperionledmappings/#{hyperion_led_mapping}/show/edit")

      assert show_live
             |> form("#hyperion_led_mapping-form", hyperion_led_mapping: @invalid_attrs)
             |> render_change() =~ "can&#39;t be blank"

      assert show_live
             |> form("#hyperion_led_mapping-form", hyperion_led_mapping: @update_attrs)
             |> render_submit()

      assert_patch(show_live, ~p"/hyperionledmappings/#{hyperion_led_mapping}")

      html = render(show_live)
      assert html =~ "Hyperion led mapping updated successfully"
    end
  end
end
