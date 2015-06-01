using System;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Xml.Serialization;

namespace G.Store.BaseTypeHelper.CompressOperation
{
    public class CompressOperation
    {
        /// <summary>
        /// 压缩XmlDocument对象为byte[]
        /// </summary>
        /// <param name="_xmldoc"></param>
        /// <returns></returns>
        public static byte[] CompressXML(XmlDocument _xmldoc)
        {
            MemoryStream temp_stream = new MemoryStream();
            _xmldoc.Save(temp_stream);

            // 生成压缩前的byte数组
            byte[] origin_bytes = new byte[(int)temp_stream.Length];
            temp_stream.Seek(0, SeekOrigin.Begin);
            temp_stream.Read(origin_bytes, 0, origin_bytes.Length);

            MemoryStream origin_stream = new MemoryStream();
            Stream zipStream;

            zipStream = new GZipStream(origin_stream, CompressionMode.Compress, true);
            zipStream.Write(origin_bytes, 0, origin_bytes.Length);
            zipStream.Close();

            byte[] compressed_bytes = origin_stream.ToArray();

            temp_stream.Close();
            origin_stream.Close();

            return compressed_bytes;
        }

        /// <summary>
        /// 将压缩为byte[]的XmlDocument对象解压
        /// </summary>
        /// <param name="_compressedbytes"></param>
        /// <returns></returns>
        public static XmlDocument UnCompressXML(byte[] _compressedbytes)
        {
            MemoryStream temp_stream = new MemoryStream();
            GZipStream zipStream = null;

            temp_stream.Write(_compressedbytes, 0, _compressedbytes.Length);
            temp_stream.Seek(0, SeekOrigin.Begin);
            zipStream = new GZipStream(temp_stream, CompressionMode.Decompress, false);

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(zipStream);

            temp_stream.Close();

            return xmldoc;
        }

        /// <summary>
        /// 压缩字节数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] CompressBinaryData(byte[] data)
        {
            MemoryStream ms = new MemoryStream();
            Stream zipStream;
            try
            {
                zipStream = new GZipStream(ms, CompressionMode.Compress, true);
                zipStream.Write(data, 0, data.Length);
                zipStream.Close();
                //  UnCompressDataSet(ms);

                byte[] resultBuffer = ms.ToArray();


                return resultBuffer;
            }
            finally
            {
                ms.Close();
            }
        }

        /// <summary>
        /// 解压缩字节数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static byte[] UnCompressBinaryData(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream();
            GZipStream zipStream = null;
            try
            {
                ms.Write(buffer, 0, buffer.Length);
                ms.Seek(0, SeekOrigin.Begin);
                zipStream = new GZipStream(ms, CompressionMode.Decompress, false);
                byte[] data = new byte[100];
                int offset = 0;
                int totalCount = 0;
                while (true)
                {
                    int bytesRead = zipStream.Read(data, offset, 100);
                    if (bytesRead == 0)
                    {
                        break;
                    }
                    //offset += bytesRead;
                    totalCount += bytesRead;
                }
                data = new byte[totalCount];
                zipStream.Read(data, 0, totalCount);
                return data;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (zipStream != null)
                    zipStream.Close();
                if (ms != null)
                    ms.Close();

            }
        }

        /// <summary>
        /// 压缩数据集
        /// </summary>
        /// <param name="ds">数据集</param>
        /// <returns></returns>
        public static byte[] CompressDataSet(DataSet ds)
        {
            string id = Guid.NewGuid().ToString().Trim();
            string filename = "XmlDoc" + id + ".xml";
            System.IO.FileStream dsStream = new System.IO.FileStream
                (filename, System.IO.FileMode.Create);

            ds.WriteXml(dsStream, XmlWriteMode.WriteSchema);

            dsStream.Position = 0;



            MemoryStream ms = new MemoryStream();
            Stream zipStream;
            try
            {
                byte[] buffer = new byte[dsStream.Length];
                dsStream.Seek(0, SeekOrigin.Begin);
                dsStream.Read(buffer, 0, buffer.Length);

                dsStream.Close();

                zipStream = new GZipStream(ms, CompressionMode.Compress, true);
                zipStream.Write(buffer, 0, buffer.Length);
                zipStream.Close();
                //  UnCompressDataSet(ms);

                byte[] resultBuffer = ms.ToArray();


                return resultBuffer;
            }
            finally
            {
                ms.Close();
                if (File.Exists(filename))
                    File.Delete(filename);
            }
        }

        /// <summary>
        /// 解压缩数据集
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <returns></returns>
        public static DataSet UnCompressDataSet(byte[] buffer)
        {

            MemoryStream ms = new MemoryStream();
            GZipStream zipStream = null;

            try
            {
                ms.Write(buffer, 0, buffer.Length);

                ms.Seek(0, SeekOrigin.Begin);
                zipStream = new GZipStream(ms, CompressionMode.Decompress, false);


                DataSet ds = new DataSet("NewDataSet");

                ds.ReadXml(zipStream, XmlReadMode.ReadSchema);
                return ds;
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (zipStream != null)
                    zipStream.Close();
                if (ms != null)
                    ms.Close();
            }
        }

        /// <summary>
        /// 解压缩数据集
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <param name="ds">数据集，将数据读到该数据集</param>
        /// <returns></returns>
        public static void UnCompressDataSet(byte[] buffer, DataSet ds)
        {

            MemoryStream ms = new MemoryStream();
            GZipStream zipStream = null;

            try
            {
                ms.Write(buffer, 0, buffer.Length);

                ms.Seek(0, SeekOrigin.Begin);
                zipStream = new GZipStream(ms, CompressionMode.Decompress, false);
                ds.Clear();
                ds.ReadXml(zipStream, XmlReadMode.ReadSchema);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (zipStream != null)
                    zipStream.Close();
                if (ms != null)
                    ms.Close();
            }
        }

        /// <summary>
        /// 压缩数据集(带状态)
        /// </summary>
        /// <param name="ds">数据集</param>
        /// <returns></returns>
        public static byte[] CompressDataSetWithState(DataSet ds)
        {
            string id = Guid.NewGuid().ToString().Trim();
            string filename = "XmlDoc" + id + ".xml";
            System.IO.FileStream dsStream = new System.IO.FileStream
                (filename, System.IO.FileMode.Create);

            ds.WriteXml(dsStream, XmlWriteMode.DiffGram);

            dsStream.Position = 0;



            MemoryStream ms = new MemoryStream();
            Stream zipStream;
            try
            {
                byte[] buffer = new byte[dsStream.Length];
                dsStream.Seek(0, SeekOrigin.Begin);
                dsStream.Read(buffer, 0, buffer.Length);

                dsStream.Close();

                zipStream = new GZipStream(ms, CompressionMode.Compress, true);
                zipStream.Write(buffer, 0, buffer.Length);
                zipStream.Close();
                //  UnCompressDataSet(ms);

                byte[] resultBuffer = ms.ToArray();


                return resultBuffer;
            }
            finally
            {
                ms.Close();
                if (File.Exists(filename))
                    File.Delete(filename);
            }
        }

        /// <summary>
        /// 解压缩数据集（带状态）
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <param name="ds">数据集，将数据读到该数据集</param>
        /// <returns></returns>
        public static void UnCompressDataSetWithState(byte[] buffer, DataSet ds)
        {

            MemoryStream ms = new MemoryStream();
            GZipStream zipStream = null;

            try
            {
                ms.Write(buffer, 0, buffer.Length);

                ms.Seek(0, SeekOrigin.Begin);
                zipStream = new GZipStream(ms, CompressionMode.Decompress, false);
                ds.Clear();
                ds.ReadXml(zipStream, XmlReadMode.DiffGram);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (zipStream != null)
                    zipStream.Close();
                if (ms != null)
                    ms.Close();
            }
        }

        /// <summary>
        /// 压缩数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static byte[] CompressData(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream();
            Stream zipStream;
            try
            {

                zipStream = new GZipStream(ms, CompressionMode.Compress, true);
                zipStream.Write(buffer, 0, buffer.Length);
                zipStream.Close();

                byte[] resultBuffer = ms.ToArray();
                return resultBuffer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ms.Close();
            }
        }

        /// <summary>
        /// 解压缩数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static byte[] UnCompressData(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream();
            GZipStream zipStream = null;
            try
            {
                ms.Write(buffer, 0, buffer.Length);
                ms.Seek(0, SeekOrigin.Begin);
                zipStream = new GZipStream(ms, CompressionMode.Decompress, false);

                MemoryStream resultStream = new MemoryStream();
                CopyStream(zipStream, resultStream);
                return resultStream.ToArray();


            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ms.Close();
            }

        }

        /// <summary>
        /// 压缩对象
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static byte[] CompressObject(object o)
        {
            string id = Guid.NewGuid().ToString().Trim();
            string filename = "XmlDoc" + id + ".xml";
            XmlSerializer mySerializer = new XmlSerializer(o.GetType());
            System.IO.FileStream dsStream = new System.IO.FileStream
                (filename, System.IO.FileMode.Create);

            mySerializer.Serialize(dsStream, o);
            dsStream.Position = 0;

            MemoryStream ms = new MemoryStream();
            Stream zipStream;
            try
            {
                byte[] buffer = new byte[dsStream.Length];
                dsStream.Seek(0, SeekOrigin.Begin);
                dsStream.Read(buffer, 0, buffer.Length);

                dsStream.Close();

                zipStream = new GZipStream(ms, CompressionMode.Compress, true);
                zipStream.Write(buffer, 0, buffer.Length);
                zipStream.Close();
                //  UnCompressDataSet(ms);

                byte[] resultBuffer = ms.ToArray();


                return resultBuffer;
            }
            finally
            {
                ms.Close();
                if (File.Exists(filename))
                    File.Delete(filename);
            }
        }

        /// <summary>
        /// 解压对象
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="typ"></param>
        /// <returns></returns>
        public static object UnCompressObject(byte[] buffer, Type typ)
        {
            MemoryStream ms = new MemoryStream();
            GZipStream zipStream = null;

            try
            {
                ms.Write(buffer, 0, buffer.Length);

                ms.Seek(0, SeekOrigin.Begin);
                zipStream = new GZipStream(ms, CompressionMode.Decompress, false);
                XmlSerializer mySerializer = new XmlSerializer(typ);
                object o = mySerializer.Deserialize(zipStream);
                return o;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (zipStream != null)
                    zipStream.Close();
                if (ms != null)
                    ms.Close();
            }
        }

        /// <summary>
        /// 复制流
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="destStream"></param>
        public static void CopyStream(GZipStream stream, Stream destStream)
        {
            // Use this method is used to read all bytes from a stream.
            byte[] quartetBuffer = new byte[4];
            int position = (int)stream.BaseStream.Length - 4;
            stream.BaseStream.Position = position;
            stream.BaseStream.Read(quartetBuffer, 0, 4);
            stream.BaseStream.Position = 0;
            int checkLength = BitConverter.ToInt32(quartetBuffer, 0);

            byte[] buffer = new byte[checkLength + 100];


            int offset = 0;
            int totalCount = 0;
            while (true)
            {
                int bytesRead = stream.Read(buffer, offset, 100);

                if (bytesRead == 0)
                {
                    break;
                }
                offset += bytesRead;
                totalCount += bytesRead;
            }

            destStream.Write(buffer, 0, totalCount);
        }

    }
}
