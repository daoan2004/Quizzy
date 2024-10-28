using System.Security.Cryptography;
using System.Text;

namespace ProjectBase.Helpers
{
    public class EncryptionHelper
    {
        public static string GetMd5Hash(string input)
        {
            // Tạo một đối tượng MD5 mới.
            using (MD5 md5Hash = MD5.Create())
            {
                // Chuyển đổi chuỗi đầu vào thành mảng byte và tính toán hash.
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Tạo một StringBuilder mới để thu thập các byte
                // và tạo ra một chuỗi.
                StringBuilder sBuilder = new StringBuilder();

                // Lặp qua từng byte của dữ liệu đã được hash 
                // và định dạng mỗi byte thành một chuỗi hex.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Trả về chuỗi hex.
                return sBuilder.ToString();
            }
        }
    }
}
