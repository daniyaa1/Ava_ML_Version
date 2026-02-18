using UnityEngine;
using System;
using System.IO;

public static class SavWav {
    public static byte[] GetWav(AudioClip clip) {
        using (var stream = new MemoryStream()) {
            using (var writer = new BinaryWriter(stream)) {
                var samples = new float[clip.samples];
                clip.GetData(samples, 0);
                Int16[] intData = new Int16[samples.Length];
                for (int i = 0; i < samples.Length; i++) {
                    intData[i] = (short)(samples[i] * 32767);
                }
                foreach (var sample in intData) writer.Write(sample);
            }
            return stream.ToArray();
        }
    }
}