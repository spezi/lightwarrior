defmodule Lightwarrior.Hyperion.Stripe do
  use Ecto.Schema
  import Ecto.Changeset

  alias Lightwarrior.Hyperion.{
    BackgroundEffect,
    Blackborderdetector,
    BoblightServer,
    Color,
    Device,
    Effects,
    FlatbufServer,
    ForegroundEffect,
    Forwarder,
    Framegrabber,
    General,
    GrabberAudio,
    GrabberV4L2,
    InstCapture,
    JsonServer,
    LedConfig,
    Leds,
    Logger,
    Network,
    ProtoServer,
    Smoothing,
    WebConfig
  }

  embedded_schema do
    embeds_one :backgroundEffect, BackgroundEffect
    embeds_one :blackborderdetector, Blackborderdetector
    embeds_one :boblightServer, BoblightServer
    embeds_one :color, Color
    embeds_one :device, Device
    embeds_one :effects, Effects
    embeds_one :flatbufServer, FlatbufServer
    embeds_one :foregroundEffect, ForegroundEffect
    embeds_one :forwarder, Forwarder
    embeds_one :framegrabber, Framegrabber
    embeds_one :general, General
    embeds_one :grabberAudio, GrabberAudio
    embeds_one :grabberV4L2, GrabberV4L2
    embeds_one :instCapture, InstCapture
    embeds_one :jsonServer, JsonServer
    embeds_one :ledConfig, LedConfig
    embeds_many :leds, Leds
    embeds_one :logger, Logger
    embeds_one :network, Network
    embeds_one :protoServer, ProtoServer
    embeds_one :smoothing, Smoothing
    embeds_one :webConfig, WebConfig
  end

  def changeset(settings, attrs) do
    settings
    |> cast(attrs, [])
    |> cast_embed(:backgroundEffect)
    |> cast_embed(:blackborderdetector)
    |> cast_embed(:boblightServer)
    |> cast_embed(:color)
    |> cast_embed(:device)
    |> cast_embed(:effects)
    |> cast_embed(:flatbufServer)
    |> cast_embed(:foregroundEffect)
    |> cast_embed(:forwarder)
    |> cast_embed(:framegrabber)
    |> cast_embed(:general)
    |> cast_embed(:grabberAudio)
    |> cast_embed(:grabberV4L2)
    |> cast_embed(:instCapture)
    |> cast_embed(:jsonServer)
    |> cast_embed(:ledConfig)
    |> cast_embed(:leds)
    |> cast_embed(:logger)
    |> cast_embed(:network)
    |> cast_embed(:protoServer)
    |> cast_embed(:smoothing)
    |> cast_embed(:webConfig)
    |> validate_required([])
  end

end

defmodule Lightwarrior.Hyperion.BackgroundEffect do
  use Ecto.Schema
  import Ecto.Changeset

  embedded_schema do
    field :color, {:array, :integer}
    field :effect, :string
    field :enable, :boolean
    field :type, :string
  end

  def changeset(background_effect, attrs) do
    background_effect
    |> cast(attrs, [:color, :effect, :enable, :type])
    |> validate_required([:color, :effect, :enable, :type])
  end
end

defmodule Lightwarrior.Hyperion.BlackBorderDetector do
  use Ecto.Schema
  import Ecto.Changeset

  embedded_schema do
    field :blurRemoveCnt, :integer
    field :borderFrameCnt, :integer
    field :enable, :boolean
    field :maxInconsistentCnt, :integer
    field :mode, :string
    field :threshold, :integer
    field :unknownFrameCnt, :integer
  end

  def changeset(black_border_detector, attrs) do
    black_border_detector
    |> cast(attrs, [:blurRemoveCnt, :borderFrameCnt, :enable, :maxInconsistentCnt, :mode, :threshold, :unknownFrameCnt])
    |> validate_required([:blurRemoveCnt, :borderFrameCnt, :enable, :maxInconsistentCnt, :mode, :threshold, :unknownFrameCnt])
  end
end

defmodule Lightwarrior.Hyperion.BoblightServer do
  use Ecto.Schema
  import Ecto.Changeset

  embedded_schema do
    field :enable, :boolean
    field :port, :integer
    field :priority, :integer
  end

  def changeset(boblight_server, attrs) do
    boblight_server
    |> cast(attrs, [:enable, :port, :priority])
    |> validate_required([:enable, :port, :priority])
  end
end

defmodule Lightwarrior.Hyperion.ColorChannelAdjustment do
  use Ecto.Schema
  import Ecto.Changeset

  embedded_schema do
    field :backlightColored, :boolean
    field :backlightThreshold, :integer
    field :blue, {:array, :integer}
    field :brightness, :integer
    field :brightnessCompensation, :integer
    field :brightnessGain, :float
    field :cyan, {:array, :integer}
    field :gammaBlue, :float
    field :gammaGreen, :float
    field :gammaRed, :float
    field :green, {:array, :integer}
    #field :id, :string
    field :leds, :string
    field :magenta, {:array, :integer}
    field :red, {:array, :integer}
    field :saturationGain, :float
    field :white, {:array, :integer}
    field :yellow, {:array, :integer}
  end

  def changeset(color_channel_adjustment, attrs) do
    color_channel_adjustment
    |> cast(attrs, [:backlightColored, :backlightThreshold, :blue, :brightness, :brightnessCompensation, :brightnessGain,
                    :cyan, :gammaBlue, :gammaGreen, :gammaRed, :green, :id, :leds, :magenta, :red, :saturationGain,
                    :white, :yellow])
    |> validate_required([:backlightColored, :backlightThreshold, :blue, :brightness, :brightnessCompensation,
                          :brightnessGain, :cyan, :gammaBlue, :gammaGreen, :gammaRed, :green, :id, :leds, :magenta,
                          :red, :saturationGain, :white, :yellow])
  end
end

defmodule Lightwarrior.Hyperion.Color do
  use Ecto.Schema
  import Ecto.Changeset

  embedded_schema do
    embeds_many :channelAdjustment, Lightwarrior.Hyperion.ColorChannelAdjustment
    field :imageToLedMappingType, :string
  end

  def changeset(color, attrs) do
    color
    |> cast(attrs, [:imageToLedMappingType])
    |> cast_embed(:channelAdjustment)
    |> validate_required([:imageToLedMappingType])
  end
end

defmodule Lightwarrior.Hyperion.Device do
  use Ecto.Schema
  import Ecto.Changeset

  embedded_schema do
    field :autoStart, :boolean
    field :colorOrder, :string
    field :enableAttempts, :integer
    field :enableAttemptsInterval, :integer
    field :hardwareLedCount, :integer
    field :host, :string
    field :latchTime, :integer
    field :max_packet, :integer
    field :port, :integer
    field :type, :string
  end

  def changeset(device, attrs) do
    device
    |> cast(attrs, [:autoStart, :colorOrder, :enableAttempts, :enableAttemptsInterval, :hardwareLedCount, :host,
                    :latchTime, :max_packet, :port, :type])
    |> validate_required([:autoStart, :colorOrder, :enableAttempts, :enableAttemptsInterval, :hardwareLedCount,
                          :host, :latchTime, :max_packet, :port, :type])
  end
end

defmodule Lightwarrior.Hyperion.Effects do
  use Ecto.Schema
  import Ecto.Changeset

  embedded_schema do
    field :disable, {:array, :string}
    field :paths, {:array, :string}
  end

  def changeset(effects, attrs) do
    effects
    |> cast(attrs, [:disable, :paths])
    |> validate_required([:disable, :paths])
  end
end

defmodule Lightwarrior.Hyperion.FlatbufServer do
  use Ecto.Schema
  import Ecto.Changeset

  embedded_schema do
    field :enable, :boolean
    field :port, :integer
    field :timeout, :integer
  end

  def changeset(flatbuf_server, attrs) do
    flatbuf_server
    |> cast(attrs, [:enable, :port, :timeout])
    |> validate_required([:enable, :port, :timeout])
  end
end

defmodule Lightwarrior.Hyperion.ForegroundEffect do
  use Ecto.Schema
  import Ecto.Changeset

  embedded_schema do
    field :color, {:array, :integer}
    field :duration_ms, :integer
    field :effect, :string
    field :enable, :boolean
    field :type, :string
  end

  def changeset(foreground_effect, attrs) do
    foreground_effect
    |> cast(attrs, [:color, :duration_ms, :effect, :enable, :type])
    |> validate_required([:color, :duration_ms, :effect, :enable, :type])
  end
end

defmodule Lightwarrior.Hyperion.Forwarder do
  use Ecto.Schema
  import Ecto.Changeset

  embedded_schema do
    field :enable, :boolean
  end

  def changeset(forwarder, attrs) do
    forwarder
    |> cast(attrs, [:enable])
    |> validate_required([:enable])
  end
end

defmodule Lightwarrior.Hyperion.Framegrabber do
  use Ecto.Schema
  import Ecto.Changeset

  embedded_schema do
    field :available_devices, :string
    field :cropBottom, :integer
    field :cropLeft, :integer
    field :cropRight, :integer
    field :cropTop, :integer
    field :device, :string
    field :device_inputs, :string
    field :enable, :boolean
    field :fps, :integer
    field :framerates, :string
    field :height, :integer
    field :input, :integer
    field :pixelDecimation, :integer
    field :resolutions, :string
    field :width, :integer
  end

  def changeset(framegrabber, attrs) do
    framegrabber
    |> cast(attrs, [:available_devices, :cropBottom, :cropLeft, :cropRight, :cropTop, :device, :device_inputs,
                    :enable, :fps, :framerates, :height, :input, :pixelDecimation, :resolutions, :width])
    |> validate_required([:available_devices, :cropBottom, :cropLeft, :cropRight, :cropTop, :device, :device_inputs,
                          :enable, :fps, :framerates, :height, :input, :pixelDecimation, :resolutions, :width])
  end
end

defmodule Lightwarrior.Hyperion.General do
  use Ecto.Schema
  import Ecto.Changeset

  embedded_schema do
    field :configVersion, :string
    field :name, :string
    field :previousVersion, :string
    field :showOptHelp, :boolean
    field :watchedVersionBranch, :string
  end

  def changeset(general, attrs) do
    general
    |> cast(attrs, [:configVersion, :name, :previousVersion, :showOptHelp, :watchedVersionBranch])
    |> validate_required([:configVersion, :name, :previousVersion, :showOptHelp, :watchedVersionBranch])
  end
end

defmodule Lightwarrior.Hyperion.GrabberAudioVuMeter do
  use Ecto.Schema
  import Ecto.Changeset

  embedded_schema do
    field :flip, :string
    field :hotColor, {:array, :integer}
    field :multiplier, :integer
    field :safeColor, {:array, :integer}
    field :safeValue, :integer
    field :tolerance, :integer
    field :warnColor, {:array, :integer}
    field :warnValue, :integer
  end

  def changeset(grabber_audio_vu_meter, attrs) do
    grabber_audio_vu_meter
    |> cast(attrs, [:flip, :hotColor, :multiplier, :safeColor, :safeValue, :tolerance, :warnColor, :warnValue])
    |> validate_required([:flip, :hotColor, :multiplier, :safeColor, :safeValue, :tolerance, :warnColor, :warnValue])
  end
end

defmodule Lightwarrior.Hyperion.GrabberAudio do
  use Ecto.Schema
  import Ecto.Changeset

  embedded_schema do
    field :audioEffect, :string
    field :device, :string
    field :enable, :boolean
    embeds_one :vuMeter, Lightwarrior.Hyperion.GrabberAudioVuMeter
  end

  def changeset(grabber_audio, attrs) do
    grabber_audio
    |> cast(attrs, [:audioEffect, :device, :enable])
    |> cast_embed(:vuMeter)
    |> validate_required([:audioEffect, :device, :enable])
  end
end

defmodule Lightwarrior.Hyperion.GrabberV4L2 do
  use Ecto.Schema
  import Ecto.Changeset

  embedded_schema do
    field :blueSignalThreshold, :integer
    field :cecDetection, :boolean
    field :cropBottom, :integer
    field :cropLeft, :integer
    field :cropRight, :integer
    field :cropTop, :integer
    field :device, :string
    field :enable, :boolean
    field :encoding, :string
    field :flip, :string
    field :fps, :integer
    field :fpsSoftwareDecimation, :integer
    field :greenSignalThreshold, :integer
    field :hardware_brightness, :integer
    field :hardware_contrast, :integer
    field :hardware_hue, :integer
    field :hardware_saturation, :integer
    field :height, :integer
    field :input, :integer
    field :noSignalCounterThreshold, :integer
    field :redSignalThreshold, :integer
    field :sDHOffsetMax, :float
    field :sDHOffsetMin, :float
    field :sDVOffsetMax, :float
    field :sDVOffsetMin, :float
    field :signalDetection, :boolean
    field :sizeDecimation, :integer
    field :width, :integer
  end

  def changeset(grabber_v4l2, attrs) do
    grabber_v4l2
    |> cast(attrs, [:blueSignalThreshold, :cecDetection, :cropBottom, :cropLeft, :cropRight, :cropTop, :device,
                    :enable, :encoding, :flip, :fps, :fpsSoftwareDecimation, :greenSignalThreshold,
                    :hardware_brightness, :hardware_contrast, :hardware_hue, :hardware_saturation, :height,
                    :input, :noSignalCounterThreshold, :redSignalThreshold, :sDHOffsetMax, :sDHOffsetMin,
                    :sDVOffsetMax, :sDVOffsetMin, :signalDetection, :sizeDecimation, :width])
    |> validate_required([:blueSignalThreshold, :cecDetection, :cropBottom, :cropLeft, :cropRight, :cropTop, :device,
                          :enable, :encoding, :flip, :fps, :fpsSoftwareDecimation, :greenSignalThreshold,
                          :hardware_brightness, :hardware_contrast, :hardware_hue, :hardware_saturation, :height,
                          :input, :noSignalCounterThreshold, :redSignalThreshold, :sDHOffsetMax, :sDHOffsetMin,
                          :sDVOffsetMax, :sDVOffsetMin, :signalDetection, :sizeDecimation, :width])
  end
end

defmodule Lightwarrior.Hyperion.InstCapture do
  use Ecto.Schema
  import Ecto.Changeset

  embedded_schema do
    field :audioEnable, :boolean
    field :audioGrabberDevice, :string
    field :audioPriority, :integer
    field :systemEnable, :boolean
    field :systemGrabberDevice, :string
    field :systemPriority, :integer
    field :v4lEnable, :boolean
    field :v4lGrabberDevice, :string
    field :v4lPriority, :integer
  end

  def changeset(inst_capture, attrs) do
    inst_capture
    |> cast(attrs, [:audioEnable, :audioGrabberDevice, :audioPriority, :systemEnable, :systemGrabberDevice,
                    :systemPriority, :v4lEnable, :v4lGrabberDevice, :v4lPriority])
    |> validate_required([:audioEnable, :audioGrabberDevice, :audioPriority, :systemEnable, :systemGrabberDevice,
                          :systemPriority, :v4lEnable, :v4lGrabberDevice, :v4lPriority])
  end
end

defmodule Lightwarrior.Hyperion.JsonServer do
  use Ecto.Schema
  import Ecto.Changeset

  embedded_schema do
    field :port, :integer
  end

  def changeset(json_server, attrs) do
    json_server
    |> cast(attrs, [:port])
    |> validate_required([:port])
  end
end

defmodule Lightwarrior.Hyperion.LedConfigClassic do
  use Ecto.Schema
  import Ecto.Changeset

  embedded_schema do
    field :bottom, :integer
    field :edgegap, :integer
    field :glength, :integer
    field :gpos, :integer
    field :hdepth, :integer
    field :left, :integer
    field :overlap, :integer
    field :pblh, :integer
    field :pblv, :integer
    field :pbrh, :integer
    field :pbrv, :integer
    field :position, :integer
    field :ptlh, :integer
    field :ptlv, :integer
    field :ptrh, :integer
    field :ptrv, :integer
    field :reverse, :boolean
    field :right, :integer
    field :top, :integer
    field :vdepth, :integer
  end

  def changeset(led_config_classic, attrs) do
    led_config_classic
    |> cast(attrs, [:bottom, :edgegap, :glength, :gpos, :hdepth, :left, :overlap, :pblh, :pblv, :pbrh, :pbrv,
                    :position, :ptlh, :ptlv, :ptrh, :ptrv, :reverse, :right, :top, :vdepth])
    |> validate_required([:bottom, :edgegap, :glength, :gpos, :hdepth, :left, :overlap, :pblh, :pblv, :pbrh,
                          :pbrv, :position, :ptlh, :ptlv, :ptrh, :ptrv, :reverse, :right, :top, :vdepth])
  end
end

defmodule Lightwarrior.Hyperion.LedConfigMatrix do
  use Ecto.Schema
  import Ecto.Changeset

  embedded_schema do
    field :cabling, :string
    field :direction, :string
    field :ledshoriz, :integer
    field :ledsvert, :integer
    field :start, :string
  end

  def changeset(led_config_matrix, attrs) do
    led_config_matrix
    |> cast(attrs, [:cabling, :direction, :ledshoriz, :ledsvert, :start])
    |> validate_required([:cabling, :direction, :ledshoriz, :ledsvert, :start])
  end
end

defmodule Lightwarrior.Hyperion.LedConfig do
  use Ecto.Schema
  import Ecto.Changeset

  embedded_schema do
    embeds_one :classic, Lightwarrior.Hyperion.LedConfigClassic
    embeds_one :matrix, Lightwarrior.Hyperion.LedConfigMatrix
  end

  def changeset(led_config, attrs) do
    led_config
    |> cast(attrs, [])
    |> cast_embed(:classic)
    |> cast_embed(:matrix)
  end
end

defmodule Lightwarrior.Hyperion.Leds do
  use Ecto.Schema
  import Ecto.Changeset

  embedded_schema do
    field :hmax, :float
    field :hmin, :float
    field :vmax, :float
    field :vmin, :float
  end

  def changeset(leds, attrs) do
    leds
    |> cast(attrs, [:hmax, :hmin, :vmax, :vmin])
    |> validate_required([:hmax, :hmin, :vmax, :vmin])
  end
end

defmodule Lightwarrior.Hyperion.Logger do
  use Ecto.Schema
  import Ecto.Changeset

  embedded_schema do
    field :level, :string
  end

  def changeset(logger, attrs) do
    logger
    |> cast(attrs, [:level])
    |> validate_required([:level])
  end
end

defmodule Lightwarrior.Hyperion.Network do
  use Ecto.Schema
  import Ecto.Changeset

  embedded_schema do
    field :apiAuth, :boolean
    field :internetAccessAPI, :boolean
    field :ipWhitelist, {:array, :string}
    field :localAdminAuth, :boolean
    field :localApiAuth, :boolean
    field :restirctedInternetAccessAPI, :boolean
  end

  def changeset(network, attrs) do
    network
    |> cast(attrs, [:apiAuth, :internetAccessAPI, :ipWhitelist, :localAdminAuth, :localApiAuth, :restirctedInternetAccessAPI])
    |> validate_required([:apiAuth, :internetAccessAPI, :ipWhitelist, :localAdminAuth, :localApiAuth, :restirctedInternetAccessAPI])
  end
end

defmodule Lightwarrior.Hyperion.ProtoServer do
  use Ecto.Schema
  import Ecto.Changeset

  embedded_schema do
    field :enable, :boolean
    field :port, :integer
    field :timeout, :integer
  end

  def changeset(proto_server, attrs) do
    proto_server
    |> cast(attrs, [:enable, :port, :timeout])
    |> validate_required([:enable, :port, :timeout])
  end
end

defmodule Lightwarrior.Hyperion.Smoothing do
  use Ecto.Schema
  import Ecto.Changeset

  embedded_schema do
    field :decay, :integer
    field :dithering, :boolean
    field :enable, :boolean
    field :interpolationRate, :integer
    field :time_ms, :integer
    field :type, :string
    field :updateDelay, :integer
    field :updateFrequency, :integer
  end

  def changeset(smoothing, attrs) do
    smoothing
    |> cast(attrs, [:decay, :dithering, :enable, :interpolationRate, :time_ms, :type, :updateDelay, :updateFrequency])
    |> validate_required([:decay, :dithering, :enable, :interpolationRate, :time_ms, :type, :updateDelay, :updateFrequency])
  end
end

defmodule Lightwarrior.Hyperion.WebConfig do
  use Ecto.Schema
  import Ecto.Changeset

  embedded_schema do
    field :crtPath, :string
    field :document_root, :string
    field :keyPassPhrase, :string
    field :keyPath, :string
    field :port, :integer
    field :sslPort, :integer
  end

  def changeset(web_config, attrs) do
    web_config
    |> cast(attrs, [:crtPath, :document_root, :keyPassPhrase, :keyPath, :port, :sslPort])
    |> validate_required([:crtPath, :document_root, :keyPassPhrase, :keyPath, :port, :sslPort])
  end
end
