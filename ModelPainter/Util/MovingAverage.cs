namespace ModelPainter.Util;

public class MovingAverage
{
	private readonly int _windowSize;
	private readonly float[] _values;

	private int _readIndex;
	private float _total;
	private float _average;
	private float _oldAverage;

	public MovingAverage(int windowSize)
	{
		_windowSize = windowSize;
		_values = new float[windowSize];
	}

	public float GetAverage()
	{
		return _average;
	}

	public float GetTotal()
	{
		return _total;
	}

	public float GetOldAverage()
	{
		return _oldAverage;
	}

	public float Update(float nextValue)
	{
		_total -= _values[_readIndex];
		_values[_readIndex] = nextValue;
		_total += _values[_readIndex];
		_readIndex++;

		if (_readIndex >= _windowSize)
		{
			_readIndex = 0;
		}

		_oldAverage = _average;
		_average = _total / _windowSize;
		return _average;
	}
}