namespace DAL
{
    public static class DatabaseSynchronization
    {
        public static void Sincronizar()
        {
            using (var context = new AppDbContext())
            {
                context.Database.Initialize(false);
            }
        }
    }
}
