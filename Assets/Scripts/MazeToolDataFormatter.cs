using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class MazeToolDataFormatter {
	private byte[] data;
	private int index=0;

	public MazeToolDataFormatter(int size) {
		data = new byte[size];
	}

	public void AddData(int i, int numBits) {
		while (numBits>0) {
			--numBits;
			data[index>>3] = (byte)(data[index>>3] | (((i>>(index/8))&1)<<(numBits%8)));
			++index;
		}
	}

	public override string ToString() {
		return System.Convert.ToBase64String(data);
	}

	public static int[] Decode(string str, int[] bitLengths) {
		// initialize vars
		int[] result = new int[bitLengths.Length];
		result.Initialize();
		byte[] data = System.Convert.FromBase64String(str);
		int index=0;

		// loop over all bits
		for (int i=0; i<bitLengths.Length; ++i) {
			for (int j=0; j<bitLengths[i]; ++j) {
				result[i] = result[i] | (data[index>>3]>>(index/8));
			}
		}
		return null;
	}
}
