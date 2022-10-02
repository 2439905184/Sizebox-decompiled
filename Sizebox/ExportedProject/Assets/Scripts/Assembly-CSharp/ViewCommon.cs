using UnityEngine;

public class ViewCommon : MonoBehaviour
{
	private int _actualPage;

	internal int page
	{
		get
		{
			int num = pageLast;
			if (_actualPage > num)
			{
				return _actualPage = num;
			}
			return _actualPage;
		}
		set
		{
			int num = pageLast;
			if (value < 0)
			{
				_actualPage = num;
			}
			else if (value > num)
			{
				_actualPage = 0;
			}
			else
			{
				_actualPage = value;
			}
			LoadPage(_actualPage);
		}
	}

	internal int pageCount
	{
		get
		{
			return CalculatePageCount();
		}
	}

	private int pageLast
	{
		get
		{
			int num = pageCount;
			if (num > 0)
			{
				num--;
			}
			return num;
		}
	}

	internal void OnNext()
	{
		page++;
	}

	internal void OnPrevious()
	{
		page--;
	}

	protected virtual int CalculatePageCount()
	{
		return 0;
	}

	protected virtual void LoadPage(int pageNumber)
	{
	}
}
