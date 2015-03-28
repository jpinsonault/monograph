using System;

namespace MonoGraph
{
	public class DuplicateVertexException : Exception
    {
        public DuplicateVertexException()
        {
        }

        public DuplicateVertexException(string message)
            : base(message)
        {
        }

        public DuplicateVertexException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class DuplicateEdgeException : Exception
    {
        public DuplicateEdgeException()
        {
        }

        public DuplicateEdgeException(string message)
            : base(message)
        {
        }

        public DuplicateEdgeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class VertexNotFoundException : Exception
    {
        public VertexNotFoundException()
        {
        }

        public VertexNotFoundException(string message)
            : base(message)
        {
        }

        public VertexNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class EdgeNotFoundException : Exception
    {
        public EdgeNotFoundException()
        {
        }

        public EdgeNotFoundException(string message)
            : base(message)
        {
        }

        public EdgeNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}