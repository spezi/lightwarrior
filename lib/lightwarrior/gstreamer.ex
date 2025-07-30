defmodule Lightwarrior.GStreamer do

  @script_path Path.expand("/home/lichtmaster/git/lightwarrior_phx18/cmd/snapshot.sh", [])

  def take_snapshot do
    case System.cmd(@script_path, [], stderr_to_stdout: true) do
      {output, 0} ->
        IO.puts("Snapshot succeeded:\n#{output}")
        :ok

      {output, exit_code} ->
        IO.warn("Snapshot failed (exit #{exit_code}):\n#{output}")
        {:error, output}
    end
  end

end
