defmodule Lightwarrior.Repo.Migrations.CreateTests do
  use Ecto.Migration

  def change do
    create table(:tests) do

      timestamps(type: :utc_datetime)
    end
  end
end
