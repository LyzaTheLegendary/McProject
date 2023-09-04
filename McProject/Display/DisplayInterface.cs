namespace Common.Console
{
    public interface DisplayInterface
    {
        public void Write(string text);
        public void Text(string text);
        public void Info(string text);
        public void Warn(string text);
        public void Error(string text);
        public void Fatal(string text);
        public void Link(string text);
    }
}
