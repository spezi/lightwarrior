defmodule LightwarriorWeb.TestLiveTest do
  use LightwarriorWeb.ConnCase

  import Phoenix.LiveViewTest
  import Lightwarrior.SpielwieseFixtures

  @create_attrs %{name: "some name"}
  @update_attrs %{name: "some updated name"}
  @invalid_attrs %{name: nil}
  defp create_test(_) do
    test = test_fixture()

    %{test: test}
  end

  describe "Index" do
    setup [:create_test]

    test "lists all tests", %{conn: conn, test: test} do
      {:ok, _index_live, html} = live(conn, ~p"/tests")

      assert html =~ "Listing Tests"
      assert html =~ test.name
    end

    test "saves new test", %{conn: conn} do
      {:ok, index_live, _html} = live(conn, ~p"/tests")

      assert {:ok, form_live, _} =
               index_live
               |> element("a", "New Test")
               |> render_click()
               |> follow_redirect(conn, ~p"/tests/new")

      assert render(form_live) =~ "New Test"

      assert form_live
             |> form("#test-form", test: @invalid_attrs)
             |> render_change() =~ "can&#39;t be blank"

      assert {:ok, index_live, _html} =
               form_live
               |> form("#test-form", test: @create_attrs)
               |> render_submit()
               |> follow_redirect(conn, ~p"/tests")

      html = render(index_live)
      assert html =~ "Test created successfully"
      assert html =~ "some name"
    end

    test "updates test in listing", %{conn: conn, test: test} do
      {:ok, index_live, _html} = live(conn, ~p"/tests")

      assert {:ok, form_live, _html} =
               index_live
               |> element("#tests-#{test.id} a", "Edit")
               |> render_click()
               |> follow_redirect(conn, ~p"/tests/#{test}/edit")

      assert render(form_live) =~ "Edit Test"

      assert form_live
             |> form("#test-form", test: @invalid_attrs)
             |> render_change() =~ "can&#39;t be blank"

      assert {:ok, index_live, _html} =
               form_live
               |> form("#test-form", test: @update_attrs)
               |> render_submit()
               |> follow_redirect(conn, ~p"/tests")

      html = render(index_live)
      assert html =~ "Test updated successfully"
      assert html =~ "some updated name"
    end

    test "deletes test in listing", %{conn: conn, test: test} do
      {:ok, index_live, _html} = live(conn, ~p"/tests")

      assert index_live |> element("#tests-#{test.id} a", "Delete") |> render_click()
      refute has_element?(index_live, "#tests-#{test.id}")
    end
  end

  describe "Show" do
    setup [:create_test]

    test "displays test", %{conn: conn, test: test} do
      {:ok, _show_live, html} = live(conn, ~p"/tests/#{test}")

      assert html =~ "Show Test"
      assert html =~ test.name
    end

    test "updates test and returns to show", %{conn: conn, test: test} do
      {:ok, show_live, _html} = live(conn, ~p"/tests/#{test}")

      assert {:ok, form_live, _} =
               show_live
               |> element("a", "Edit")
               |> render_click()
               |> follow_redirect(conn, ~p"/tests/#{test}/edit?return_to=show")

      assert render(form_live) =~ "Edit Test"

      assert form_live
             |> form("#test-form", test: @invalid_attrs)
             |> render_change() =~ "can&#39;t be blank"

      assert {:ok, show_live, _html} =
               form_live
               |> form("#test-form", test: @update_attrs)
               |> render_submit()
               |> follow_redirect(conn, ~p"/tests/#{test}")

      html = render(show_live)
      assert html =~ "Test updated successfully"
      assert html =~ "some updated name"
    end
  end
end
