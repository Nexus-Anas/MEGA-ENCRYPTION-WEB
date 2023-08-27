using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.AspNetCore.Http;

namespace MEGA_ENCRYPTION_WEB.Data
{
    public class Cipher
    {/////////////////////////////////////Encryption///////////////////////////////////////////

        private static byte[] _abc;
        private static byte[,] _table;



        public static async Task<byte[]> EncryptFileAsync(byte[] data, string psw)
        {
            // Initialize abc and table
            _abc = new byte[256];
            for (int i = 0; i < 256; i++)
                _abc[i] = Convert.ToByte(i);

            _table = new byte[256, 256];
            for (int i = 0; i < 256; i++)
                for (int j = 0; j < 256; j++)
                    _table[i, j] = _abc[(i + j) % 256];

            try
            {
                var pswTmp = Encoding.ASCII.GetBytes(psw);
                var keys = new byte[data.Length];
                var result = new byte[data.Length];
                for (int i = 0; i < data.Length; i++)
                    keys[i] = pswTmp[i % pswTmp.Length];

                // Encrypt asynchronously
                await Task.Run(() =>
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        var value = data[i];
                        var key = keys[i];
                        int valueIndex = -1, keyIndex = -1;
                        for (int j = 0; j < 256; j++)
                            if (_abc[j] == value)
                            {
                                valueIndex = j;
                                break;
                            }
                        for (int j = 0; j < 256; j++)
                            if (_abc[j] == key)
                            {
                                keyIndex = j;
                                break;
                            }
                        result[i] = _table[keyIndex, valueIndex];
                    }
                });

                return result;
            }
            catch (Exception)
            {
                // Handle exceptions here
                return null;
            }
        }







        public static async Task<byte[]> DecryptBytesAsync(byte[] encryptedData, string psw)
        {
            // Initialize abc and table
            _abc = new byte[256];
            for (int i = 0; i < 256; i++)
                _abc[i] = Convert.ToByte(i);

            _table = new byte[256, 256];
            for (int i = 0; i < 256; i++)
                for (int j = 0; j < 256; j++)
                    _table[i, j] = _abc[(i + j) % 256];

            try
            {
                var pswTmp = Encoding.ASCII.GetBytes(psw);
                var keys = new byte[encryptedData.Length];
                var result = new byte[encryptedData.Length];
                for (int i = 0; i < encryptedData.Length; i++)
                    keys[i] = pswTmp[i % pswTmp.Length];

                // Decrypt asynchronously
                await Task.Run(() =>
                {
                    for (int i = 0; i < encryptedData.Length; i++)
                    {
                        var value = encryptedData[i];
                        var key = keys[i];
                        int valueIndex = -1, keyIndex = -1;
                        for (int j = 0; j < 256; j++)
                            if (_abc[j] == key)
                            {
                                keyIndex = j;
                                break;
                            }
                        for (int j = 0; j < 256; j++)
                            if (_table[keyIndex, j] == value)
                            {
                                valueIndex = j;
                                break;
                            }
                        result[i] = _abc[valueIndex];
                    }
                });

                return result;
            }
            catch (Exception)
            {
                // Handle exceptions here
                return null;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////

    }
}
