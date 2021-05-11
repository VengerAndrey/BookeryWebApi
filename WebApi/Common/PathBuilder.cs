using System;

namespace WebApi.Common
{
    public class PathBuilder
    {
        private string _path = "";

        public string GetPath() => _path;

        public void Reset()
        {
            _path = "";
        }

        public void AddNode(string name)
        {
            if (_path == "")
            {
                _path += name;
            }
            else
            {
                _path += $"/{name}";
            }
        }

        public string GetTopNode()
        {
            string topNode;

            if (!_path.Contains("/"))
            {
                topNode = _path;
                _path = "";
            }
            else
            {
                topNode = _path.Substring(0, _path.IndexOf("/"));
                _path = _path.Substring(_path.IndexOf("/") + 1, _path.Length - _path.IndexOf("/") - 1);
            }

            return topNode;
        }

        public void ParseUri(Uri uri)
        {
            _path = "";

            if (uri is null)
            {
                return;
            }

            if (uri.Segments.Length <= 1)
            {
                _path = "";
            }
            else
            {
                for (int i = 1; i < uri.Segments.Length; i++)
                {
                    _path += uri.Segments[i];
                }
            }
        }

        public void ParsePath(string path)
        {
            _path = path;
        }

        public bool IsFile() => _path.Contains(".");

        public string GetFileName() =>
            _path.Substring(_path.LastIndexOf("/") + 1, _path.Length - _path.LastIndexOf("/") - 1);

        public bool IsFileLeft()
        {
            return !_path.Contains("/") && _path.Contains(".");
        }
    }
}
