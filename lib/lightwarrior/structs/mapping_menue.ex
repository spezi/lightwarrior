defmodule Lightwarrior.MappingMenueForm do
  #defstruct stripe_length: 0, opacity: 100
  use Ecto.Schema
  import Ecto.Changeset

  embedded_schema do
    field :side, :string
    field :stripe_length, :float
    field :opacity, :integer
    field :lockdistance, :boolean
  end


  def changeset(menue_mapping_form, params \\ %{}) do
    menue_mapping_form
    |> cast(params, [:side, :stripe_length, :opacity, :lockdistance])
    |> validate_required([:side, :opacity, :lockdistance])
    #|> validate_format(:email, ~r/@/)
    #|> validate_inclusion(:age, 18..100)
    #|> unique_constraint(:email)
  end

end
