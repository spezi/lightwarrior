defmodule Lightwarrior.Imageplayer.IPlayer do
  use Ecto.Schema
  import Ecto.Changeset

  embedded_schema do
    field :name, :string
    field :file, :string
    field :thumbnail_path, :string
    field :type, :string
    field :output_type, :string
    field :status, :string
  end

  def changeset(iplayer, attrs) do
    iplayer
    |> cast(attrs, [:name, :file, :thumbnail_path, :output_type, :status])
    |> validate_inclusion(:output_type, ["autovideosink", "shmdatasink"])
  end

end
