using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Extentions
{
    public static class IDGenerator
    {
        private static readonly char _minChar = 'A';
        private static readonly char _maxChar = 'Z';
        private static readonly int _minDigit = 1;
        private static readonly int _maxDigit = 999;
        private static int _fixedLength = 4;//zero means variable length
        private static int _currentDigit = 1;
        private static string _currentBase = "A";
        #region [A001 - Z999]
        //public static string NextID()
        //{
        //    if (_currentBase[_currentBase.Length - 1] <= _maxChar)
        //    {
        //        if (_currentDigit <= _maxDigit)
        //        {
        //            var result = string.Empty;
        //            if (_fixedLength > 0)
        //            {
        //                var prefixZeroCount = _fixedLength - _currentBase.Length;
        //                if (prefixZeroCount < _currentDigit.ToString().Length)
        //                    throw new InvalidOperationException("The maximum length possible has been exeeded.");
        //                result = result = _currentBase + _currentDigit.ToString("D" + prefixZeroCount.ToString());
        //            }
        //            else
        //            {
        //                result = _currentBase + _currentDigit.ToString();
        //            }
        //            _currentDigit++;
        //            return result;
        //        }
        //        else
        //        {
        //            _currentDigit = _minDigit;
        //            if (_currentBase[_currentBase.Length - 1] == _maxChar)
        //            {
        //                _currentBase = _currentBase.Remove(_currentBase.Length - 1) + _minChar;
        //                _currentBase += _minChar.ToString();
        //            }
        //            else
        //            {
        //                var newChar = _currentBase[_currentBase.Length - 1];
        //                newChar++;
        //                _currentBase = _currentBase.Remove(_currentBase.Length - 1) + newChar.ToString();
        //            }

        //            return NextID();
        //        }
        //    }
        //    else
        //    {
        //        _currentDigit = _minDigit;
        //        _currentBase += _minChar.ToString();
        //        return NextID();

        //    }
        //}

        //public static string NextID(string currentId)
        //{
        //    if (string.IsNullOrWhiteSpace(currentId))
        //        return NextID();

        //    var charCount = currentId.Length;
        //    var indexFound = -1;
        //    for (int i = 0; i < charCount; i++)
        //    {
        //        if (!char.IsNumber(currentId[i]))
        //            continue;

        //        indexFound = i;
        //        break;
        //    }
        //    if (indexFound > -1)
        //    {
        //        _currentBase = currentId.Substring(0, indexFound);
        //        _currentDigit = int.Parse(currentId.Substring(indexFound)) + 1;
        //    }
        //    return NextID();
        //}
        #endregion


        #region [001A - 999Z]
        public static string NextID()
        {
            if (_currentBase[_currentBase.Length - 1] <= _maxChar)
            {
                _currentDigit++;
                if (_currentDigit <= _maxDigit)
                {
                    var result = string.Empty;
                    if (_fixedLength > 0)
                    {
                        var prefixZeroCount = _fixedLength - _currentBase.Length;
                        if (prefixZeroCount < _currentDigit.ToString().Length)
                            throw new InvalidOperationException("The maximum length possible has been exeeded.");
                        result = result = _currentDigit.ToString("D" + prefixZeroCount.ToString()) + _currentBase;
                    }
                    else
                    {
                        result = _currentDigit.ToString() + _currentBase;
                    }

                    return result;
                }
                else
                {
                    _currentDigit = 0;
                    if (_currentBase[_currentBase.Length - 1] == _maxChar)
                    {
                        _currentBase = _currentBase.Remove(_currentBase.Length - 1) + _minChar;
                        _currentBase += _minChar.ToString();
                    }
                    else
                    {
                        var newChar = _currentBase[_currentBase.Length - 1];
                        newChar++;
                        _currentBase = _currentBase.Remove(_currentBase.Length - 1) + newChar.ToString();
                    }

                    return NextID();
                }
            }
            else
            {
                _currentDigit = _minDigit;
                _currentBase += _minChar.ToString();
                return NextID();

            }
        }

        public static string NextID(string currentId)
        {
            if (string.IsNullOrWhiteSpace(currentId))
            {
                _currentDigit = 0;
                return NextID();
            }
            else
            {

                var charCount = currentId.Length;
                var indexFound = -1;
                for (int i = 0; i < charCount; i++)
                {
                    if (char.IsNumber(currentId[i]))
                        continue;

                    indexFound = i;
                    break;
                }
                if (indexFound > -1)
                {
                    _currentBase = currentId.Substring(indexFound, 4 - indexFound);
                    _currentDigit = int.Parse(currentId.Substring(0, indexFound));
                }
                return NextID();
            }
        }
        #endregion
    }
}
