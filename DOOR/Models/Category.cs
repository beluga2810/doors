﻿namespace DOOR.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Door> Doors { get; set; }
    }
}
