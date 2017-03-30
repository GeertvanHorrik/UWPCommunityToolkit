﻿// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************

using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Html;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Microsoft.Toolkit.Uwp
{
    /// <summary>
    /// This class can set clipboard format easier.
    /// </summary>
    public static class ClipboardHelper
    {
        /// <summary>
        /// Get image bytes from clipboard.
        /// </summary>
        /// <returns>The image bytes.</returns>
        public static async Task<byte[]> GetImageAsync()
        {
            var dataPackageView = Clipboard.GetContent();
            if (dataPackageView.Contains(StandardDataFormats.Bitmap))
            {
                var imageReceived = await dataPackageView.GetBitmapAsync();
                using (var imageStream = await imageReceived.OpenReadAsync())
                {
                    var bytes = new byte[imageStream.Size];
                    await imageStream.ReadAsync(bytes.AsBuffer(), (uint)imageStream.Size, InputStreamOptions.None);
                    return bytes;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get html from clipboard.
        /// </summary>
        /// <returns>The raw html string.</returns>
        public static async Task<string> GetRawHtmlAsync()
        {
            var dataPackageView = Clipboard.GetContent();
            if (dataPackageView.Contains(StandardDataFormats.Html))
            {
                string htmlFormat;
                try
                {
                    htmlFormat = await dataPackageView.GetHtmlFormatAsync();
                }
                catch (ArgumentException)
                {
                    // if the clipboard html format is empty string.
                    return null;
                }
                return HtmlFormatHelper.GetStaticFragment(htmlFormat);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get rtf format text from clipboard.
        /// </summary>
        /// <returns>The rtf format text.</returns>
        public static async Task<string> GetRtfAsync()
        {
            var dataPackageView = Clipboard.GetContent();
            if (dataPackageView.Contains(StandardDataFormats.Rtf))
            {
                try
                {
                    return await dataPackageView.GetRtfAsync();
                }
                catch (ArgumentException)
                {
                    // if the clipboard rtf is empty string.
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get text from clipboard.
        /// </summary>
        /// <returns>The text string.</returns>
        public static async Task<string> GetTextAsync()
        {
            var dataPackageView = Clipboard.GetContent();
            if (dataPackageView.Contains(StandardDataFormats.Text))
            {
                return await dataPackageView.GetTextAsync();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Set image bytes into clipboard.
        /// </summary>
        /// <param name="image">The image bytes.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
        public static async Task SetImageAsync(byte[] image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            var dataPackage = new DataPackage();

            var tempFileName = string.Format("{0}.tmp", Guid.NewGuid());
            var tempFile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(tempFileName, CreationCollisionOption.GenerateUniqueName);
            await FileIO.WriteBytesAsync(tempFile, image);
            var randomAccessStreamReference = RandomAccessStreamReference.CreateFromFile(tempFile);
            dataPackage.SetBitmap(randomAccessStreamReference);
            await tempFile.DeleteAsync(StorageDeleteOption.PermanentDelete);

            Clipboard.SetContent(dataPackage);
            Clipboard.Flush();
        }

        /// <summary>
        /// Set html into clipboard.
        /// </summary>
        /// <param name="html">The html string.</param>
        /// <exception cref="ArgumentNullException">'html' is null.</exception>
        public static void SetRawHtml(string html)
        {
            if (html == null)
            {
                throw new ArgumentNullException(nameof(html));
            }

            var dataPackage = new DataPackage();
            var htmlFormat = HtmlFormatHelper.CreateHtmlFormat(html);
            var plainText = HtmlUtilities.ConvertToText(html);
            dataPackage.SetHtmlFormat(htmlFormat);
            dataPackage.SetText(plainText);
            Clipboard.SetContent(dataPackage);
            Clipboard.Flush();
        }

        /// <summary>
        /// Set rtf format text into clipboard.
        /// </summary>
        /// <param name="rtf">The rtf format text.</param>
        /// <exception cref="ArgumentNullException">'rtf' is null.</exception>
        public static void SetRtf(string rtf)
        {
            if (rtf == null)
            {
                throw new ArgumentNullException(nameof(rtf));
            }

            var dataPackage = new DataPackage();
            dataPackage.SetRtf(rtf);
            Clipboard.SetContent(dataPackage);
            Clipboard.Flush();
        }

        /// <summary>
        /// Set text into clipboard.
        /// </summary>
        /// <param name="text">The text string.</param>
        /// <exception cref="ArgumentNullException">'text' is null.</exception>
        public static void SetText(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            var dataPackage = new DataPackage();
            dataPackage.SetText(text);
            Clipboard.SetContent(dataPackage);
            Clipboard.Flush();
        }
    }
}