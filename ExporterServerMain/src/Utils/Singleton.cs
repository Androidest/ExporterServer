namespace Utils
{
    public abstract class Singleton<T> where T : new()
    {
        protected Singleton()
        {
            InitInstance();
        }

        public virtual void InitInstance()
        {

        }

        private static T _instance;

        public static T Instance => Nested<T>.Instance;

        private class Nested<T> where T : new()
        {
            static Nested()
            {
            }

            public static readonly T Instance = new T();
        }
    }
}