// ConsoleApplication2.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "stdafx.cpp"
#include <conio.h>

struct st1 {
	int value;
	int freq;
};

void sort(int *a)
{
	for (int i = 0; i < 10-1; i++)
	{
		for (int j = 0; j < 10-1; j++)
		{
			if (a[j] > a[j + 1])
			{
				int tmp = a[j];
				a[j] = a[j+1];
				a[j + 1] = tmp;
			}
		}
	}
}

void _print(int *a)
{
	for (int i = 0; i < 10; i++)
	{
		cout << a[i] << " ";
	}
	cout << endl;
}

void MaxFreq(int a[],int n)
{
	st1 *check = new st1[n];
	for (int i = 0; i < n; i++)
	{
		check[i].value = 0;
		check[i].freq = 0;
	}
	int count = 0;
	int maxfreq;
	for (int i = 0; i < n-1; i++)
	{
		check[count].value = a[i];
		check[count].freq++;
		while (a[i+1] == a[i])
		{
			check[count].freq++;
			i++;
		}
		count++;
	}
	maxfreq = check[0].freq;
	for (int i = 1; i < count; i++)
	{
		if (check[i].freq > maxfreq)
		{
			maxfreq = check[i].freq;
		}
	}
	for (int i = 0; i < count; i++)
	{
		if (check[i].freq == maxfreq)
		{
			cout << check[i].value << "-" << check[i].freq << "  ";
		}
	}
	/*int max = a[0];
	int maxcount = 1;
	for (int i = 0; i < 9; i++)
	{
		int tmp = 1;
		while (a[i+1] == a[i])
		{
			tmp++;
			i++;
			if (tmp > maxcount)
			{
				maxcount = tmp;
				max = a[i];
			}
		}
	}
	cout << max << "  " << maxcount;*/
}

int main()
{
	//int a, b, c;
	//
	//while (true)
	//{
	//	cin >> a >> b >> c;
	//	if (a < b)
	//	{
	//		if (b < c)
	//		{
	//			cout << a << b << c << endl;
	//		}
	//		else if (c > a)
	//			cout << a << c << b << endl;
	//		else
	//			cout << c << a << b << endl;
	//	}
	//	else
	//		if (b > c)
	//		{
	//			cout << c << b << a << endl;
	//		}
	//		else if (c > a)
	//			cout << b << a << c << endl;
	//		else
	//			cout << b << c << a << endl;
	//}
	/*int n = 123456;
	int count = 0;*/
	/*while (n > 0)
	{
		n = n/10;
		count++;
	}*/
	/*int result = 0;
	while (n>0)
	{
		result = result * 10 + n % 10;
		n /= 10;
	}
	char* a = "";*/
	//cout << result << endl; 
	/*int x = sizeof(a);
	int y = strlen(a);

	for (int i = 0; i < strlen(a)-1; i++)
	{
		if (*(a + i) != ' ' && *(a+i+1) == ' ')
		{
			count++;
		}
	}
	if (*(a + strlen(a) - 1) != ' ')
	{
		count++;
	}
	cout << count << endl;*/

	int a[10] = { 0,2,1,5,5,6,6,2,9,1 };
	sort(a);
	_print(a);
	MaxFreq(a,10);
	_getch();
    return 0;
}

