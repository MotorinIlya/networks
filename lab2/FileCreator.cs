using System.Text;

namespace lab2
{
    class FileCreator
    {
        string name;
        string directory;
        public FileCreator (string name, string directory)
        {
            this.name = name;
            this.directory = directory;
        }

        public string createName ()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(directory).Append("/uploads/").Append(Path.GetFileName(name));
            renameFile(builder);
            return builder.ToString();
        }

        void renameFile (StringBuilder builder)
        {
            int ind = 0;
            for (int i = builder.Length - 1; i >= 0; i--)
            {
                if (builder[i] == '.')
                {
                    ind = i;
                    break;
                }
            }

            bool isFirst = false;
            int count = 0;
            while (File.Exists(builder.ToString())) 
            {
                if (!isFirst)
                {
                    builder.Insert(ind, $"{count}");
                    isFirst = true;
                }
                else
                {   
                    builder.Remove(ind, count.ToString().Length);
                    count++;
                    builder.Insert(ind, $"{count}");
                }
                
            }
        }
    }
}