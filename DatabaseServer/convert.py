import math
import numpy as np
from scipy.fft import fft, fftfreq

# 0 -> fft
# 2 -> raw_charts
def convert_spectra(timeline, end_freq, raw):
    d = (np.tile(np.arange(timeline), math.ceil(len(raw['rawdata']) / timeline)) + 1)[:len(raw['rawdata'])]
    n6 = 2 * math.pi * end_freq / (timeline * 1000)
    print(n6)
    print(len(raw['rawdata']))
    print(len(d))
    p = np.array(raw['rawdata']) * raw['scale_factor'] / math.sqrt(2) / (n6 * d) / 25.4 * 10
    return ((np.arange(len(raw['rawdata'])) / len(raw['rawdata']) * end_freq).tolist(), p.tolist())
    # plt.xticks(np.arange(0, 1000, step=25))


