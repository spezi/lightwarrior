defmodule Lightwarrior.Imageplayer.IPlayer do
  use Ecto.Schema
  import Ecto.Changeset

  def changeset(settings, attrs) do
    settings
    |> cast(attrs, [])
    |> validate_required([])
  end

end
