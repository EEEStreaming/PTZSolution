using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PTZPadController.PresentationLayer
{
    static class PresetIconList
    {
        public static List<PresetIcon> Icons;

        static PresetIconList()
        {
            var currentDir = System.IO.Path.GetDirectoryName(Application.ResourceAssembly.Location);
            currentDir = System.IO.Path.Combine(currentDir, "PresetIcons");

            string[] filePaths = Directory.GetFiles(currentDir, "*.png");
            PresetIcon icon;
            Icons = new List<PresetIcon>();
            foreach (var file in filePaths)
            {
                icon = new PresetIcon();
                icon.FullPath = file;
                icon.Key = System.IO.Path.GetFileNameWithoutExtension(file);
                Icons.Add(icon);
            }
        }
    }

   public class PresetIcon
    {
        private BitmapSource m_BitmapImage;

        public string Key { get; set; }

        public string FullPath { get; set; }


        public BitmapSource UriSource
        {
            get
            {
                if (File.Exists(FullPath))
                {
                    if (m_BitmapImage == null)
                    {
                        //tentative de creation du thumbnail via Bitmap frame : très rapide et très peu couteux en mémoire !
                        BitmapFrame frame = BitmapFrame.Create(
                            new Uri(FullPath),
                            BitmapCreateOptions.DelayCreation,
                            BitmapCacheOption.None);
                        if (frame.Thumbnail == null) //echec, on tente avec BitmapImage (plus lent et couteux en mémoire)
                        {
                            var image = new BitmapImage();
                            image.BeginInit();
                            image.DecodePixelWidth = 64;
                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.CreateOptions = BitmapCreateOptions.DelayCreation;
                            image.UriSource = new Uri(FullPath);
                            image.EndInit();

                            if (image.CanFreeze) //pour éviter les memory leak
                                image.Freeze();

                            m_BitmapImage = image;
                        }
                        else
                        {
                            //récupération des métas de l'image
                            //var meta = frame.Metadata as BitmapMetadata;
                            m_BitmapImage = frame.Thumbnail;
                        }
                    }
                    return m_BitmapImage;
                }
                return null;
            }
        }

        public PresetIcon()
        {

        }
    }


}
