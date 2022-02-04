using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public class DogRepository : IDogRepository
    {
        private readonly IConfiguration _config;

        // The constructor accepts an IConfiguration object as a parameter. This class comes from the ASP.NET framework and is useful for retrieving things out of the appsettings.json file like connection strings.
        public DogRepository(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        //Get list of all dogs
        public List<Dog> GetAllDogs()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, [Name], OwnerId, Breed, Notes, ImageURL
                        FROM Dog
                    ";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Dog> dogs = new List<Dog>();

                    while (reader.Read())
                    {
                        Dog dog= new Dog
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                            Breed = reader.GetString(reader.GetOrdinal("Breed")),
                           
                        };
                        if (!reader.IsDBNull(reader.GetOrdinal("Notes")))
                        {
                            dog.Notes = reader.GetString(reader.GetOrdinal("Notes"));
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("ImageURL")))
                        {
                            dog.ImageURL = reader.GetString(reader.GetOrdinal("ImageURL"));
                        }

                        dogs.Add(dog);
                    }

                    reader.Close();

                    return dogs;
                }
            }
        }

        //Add a new dog
        public void AddDog(Dog dog)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                INSERT INTO Dog ([Name], OwnerId, Breed, Notes, ImageUrl)
                OUTPUT INSERTED.ID
                VALUES (@name, @ownerId, @breed, @notes, @imageUrl);
            ";

                    cmd.Parameters.AddWithValue("@name", dog.Name);
                    cmd.Parameters.AddWithValue("@breed", dog.Breed);
                    cmd.Parameters.AddWithValue("@ownerId", dog.OwnerId);

                    // nullable columns
                    cmd.Parameters.AddWithValue("@notes", dog.Notes ?? "");
                    cmd.Parameters.AddWithValue("@imageUrl", dog.ImageURL ?? "");

                    int newlyCreatedId = (int)cmd.ExecuteScalar();

                    dog.Id = newlyCreatedId;

                }
            }
        }
        //public void AddDog(Dog dog)
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"
        //            INSERT INTO Dog ([Name], OwnerId, Breed)
        //            VALUES ('@name', @ownerId, '@breed');
        //        ";

        //            cmd.Parameters.AddWithValue("@name", dog.Name);
        //            cmd.Parameters.AddWithValue("@ownerId", dog.OwnerId);
        //            cmd.Parameters.AddWithValue("@breed", dog.Breed);
        //            cmd.Parameters.AddWithValue("@notes", dog.Notes);
        //            cmd.Parameters.AddWithValue("@imageUrl", dog.ImageURL);

        //            cmd.ExecuteNonQuery();

        //            //dog.Id = id;
        //        }
        //    }
        //}

        //Get a dog by ID
        public Dog GetDogById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, [Name], OwnerId, Breed
                        FROM Dog
                        WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                       Dog dog = new Dog()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                            Breed = reader.GetString(reader.GetOrdinal("Breed"))
                           
                        };

                        reader.Close();
                        return dog;
                    }

                    reader.Close();
                    return null;
                }
            }
        }


        //Edit (update) a dog

        public void UpdateDog(Dog dog)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            UPDATE Dog
                            SET 
                                [Name] = @name, 
                                OwnerId = @ownerId, 
                                Breed = @breed
                                
                            WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@name", dog.Name);
                    cmd.Parameters.AddWithValue("@ownerId", dog.OwnerId);
                    cmd.Parameters.AddWithValue("@breed", dog.Breed);
                    cmd.Parameters.AddWithValue("@id", dog.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }


        //DELETE DOG

        public void DeleteDog(int dogId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            DELETE FROM Dog
                            WHERE Id = @id
                        ";

                    cmd.Parameters.AddWithValue("@id", dogId);

                    cmd.ExecuteNonQuery();
                }
            }
        }



        //Get a list of dogs of a particular owner
        public List<Dog> GetDogsByOwnerId(int ownerId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                SELECT Id, Name, Breed, Notes, ImageUrl, OwnerId 
                FROM Dog
                WHERE OwnerId = @ownerId
            ";

                    cmd.Parameters.AddWithValue("@ownerId", ownerId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Dog> dogs = new List<Dog>();

                    while (reader.Read())
                    {
                        Dog dog = new Dog()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Breed = reader.GetString(reader.GetOrdinal("Breed")),
                            OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId"))
                        };

                        // Check if optional columns are null
                        if (reader.IsDBNull(reader.GetOrdinal("Notes")) == false)
                        {
                            dog.Notes = reader.GetString(reader.GetOrdinal("Notes"));
                        }
                        if (reader.IsDBNull(reader.GetOrdinal("ImageUrl")) == false)
                        {
                            dog.ImageURL = reader.GetString(reader.GetOrdinal("ImageUrl"));
                        }

                        dogs.Add(dog);
                    }
                    reader.Close();
                    return dogs;
                }
            }
        }

    }
}