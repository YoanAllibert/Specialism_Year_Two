# **Frequency Detection Package: Specialism Brief 1**

## *Introduction*

The *frequency* available for humans to hear usually range from about 20 Hz to 20 kHz (20 000 Hz). In Unity, we are able to *sample* any audio track, and get a result of the *frequency's amplitude* at this particular sample. This package provide a way to choose four *bands* of frequency (a range) and, depending on a threshold that can be customise for each band, will turn On and Off a GameObject. It can easily be extended to generate level data from an audio track.

**Note**: Samples can be seen as *frequency resolution*, and calculate the *relative amplitude* at their point. With a 1024 array, we have a resolution of 23.4 hertz, calculated by the total *frequency* range (24kHz) divided by the sample count: 24 000 / 1024. In this package we sample the relative amplitude every 23.4 hertz. This can be change through the script for higher resolution but keep in mind that it needs to be a power of two.

The FFTWindow parameter stands for `Fast Fourier Transfer Window`, FFT being an *algorythm* to convert a signal from *original* domain to *frequency* domain. Unity have some predefined *FFTWindow* to help reduce leakage of signal across the frequency band.

## *Behaviour*

[`FrequencyDetection`]

## *FrequencyDetection*

This *script* can be added to *any* Gameobject. It is recommended to create an empty objectfor this behaviour only.
Here is a breakdown of the parameters available:

## *Parameters*

### Audio Source And Visual

The first *field* to populate is the **source Audio** that will be sample. Simply drag and drop the *Audio Source* of any other Gameobject into this slot.

The array of **Objects** represent the four *GameObjects* that will be turned On and Off if the frequency is detected. They are set in order, so the *Element 0* correspond to *Frequency Band One*, *Element 1* to *Frequency Band Two* etc. It is essential that *exactly* four Gameobjects are being passed in the array.

### Frequency Bands

Here, we are going to choose the **Range** and **Threshold** for each *frequency band*. 

The parameters *Frequency From* and *Frequency To* are respectively the *beginning* and *end* of the band. They range according to the human perception of sound: a minimum of 20 Hz and maximum of 20 kHz.

The *threshold* is the average amount a frequency band has to reach in order to be detected. Here, we usually want to use *different* amount depending on the range of frequency. A rule of thumb is that *higher* frequency have *lower* amplitude, so the resulting threshold tend to be *smaller*, and on the opposite, *lower* frequency usually require *higher* threshold. Testing different values is important to get to the desired result.

## How to Choose Range

The audio spectrum is commonly devided into **seven** bands, acting on the total sound. They are divided as follow:
| Frequency Range | Frequency Values |
|-----------------|------------------|
| Sub-bass        | 20 to 60 Hz      |
| Bass            | 60 to 250 Hz     |
| Low midrange 	  | 250 to 500 Hz 	 |
| Midrange        | 500 Hz to 2 kHz  |
| Upper midrange  | 2 to 4 kHz       |
| Presence        | 4 to 6 kHz       |
| Brilliance      | 6 to 20 kHz      |

*Source*: [teachmeaudio.com](https://www.teachmeaudio.com/mixing/techniques/audio-spectrum/ "Audio Spectrum")

You can then choose a range that represent the whole band, or just a part of it. It is recommended not to choose a range in-between two band, as it does not feel so natural to the human hear. See the example scene for values representing *Bass*, *Low midrange*, *Midrange* and *Presence*, using their respective arbitrary threshold.

[`FrequencyDetection`]: #FrequencyDetection