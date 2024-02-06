namespace Neural_Network
{
    class ConvertingErrorException : Exception
    {
        public ConvertingErrorException(string message) : base(message) { }
    }
    class FileException : Exception
    {
        public FileException(string message) : base(message) { }
    }
}
