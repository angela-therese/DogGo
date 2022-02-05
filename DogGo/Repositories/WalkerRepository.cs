using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public class WalkerRepository : IWalkerRepository
    {
        private readonly IConfiguration _config;

        // The constructor accepts an IConfiguration object as a parameter. This class comes from the ASP.NET framework and is useful for retrieving things out of the appsettings.json file like connection strings.
        public WalkerRepository(IConfiguration config)
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

        public List<Walker> GetAllWalkers()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, [Name], ImageUrl, NeighborhoodId
                        FROM Walker
                    ";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Walker> walkers = new List<Walker>();
                    while (reader.Read())
                    {
                        Walker walker = new Walker
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl")),
                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId"))
                        };

                        walkers.Add(walker);
                    }

                    reader.Close();

                    return walkers;
                }
            }
        }

        public Walker GetWalkerById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT w.Id AS WalkerId,
                               w.Name AS WalkerName, 
                                w.NeighborhoodId, 
                                w.ImageURL AS WalkerImage, 
                                k.Id AS WalkId, 
                                k.Date, 
                                k.Duration, 
                                k.WalkerId AS WalkWalkerId,
                                d.Id AS DogId, 
                                d.Name AS DogName,
                                d.OwnerId AS DogOwnerId,
                                o.Id AS OwnerId, 
                                o.Name AS OwnerName
                                From Walker w
                                LEFT JOIN Walks k on k.WalkerId = w.Id 
                                LEFT JOIN Dog d on k.DogId = d.Id
                                LEFT JOIN Owner o on d.ownerId = o.Id

                                  WHERE w.Id = @id
                                     ";

                    cmd.Parameters.AddWithValue("@id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    //Walker walker = new Walker()
                    //{
                    //    Id = 0
                    //};
                    Walker walker = new Walker();

                    while (reader.Read())
                        

                    {
                        

                        if (walker.Id == 0)
                        {
                            {
                                walker.Id = reader.GetInt32(reader.GetOrdinal("WalkerId"));
                                walker.Name = reader.GetString(reader.GetOrdinal("WalkerName"));
                                walker.ImageUrl = reader.GetString(reader.GetOrdinal("WalkerImage"));
                                walker.NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId"));
                                walker.Walks = new List<Walk>();

                            };

                            if (!reader.IsDBNull(reader.GetOrdinal("WalkId")))

                            {


                                //{
                                Walk walk = new Walk
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("WalkId")),
                                    Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                                    Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                                    Client = reader.GetString(reader.GetOrdinal("OwnerName"))
                                };

                                walker.Walks.Add(walk);
                                
                            }
                        }
                        //added else statement
                        else
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("WalkId")))

                            {

                                //{
                                Walk walk = new Walk
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("WalkId")),
                                    Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                                    Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                                    Client = reader.GetString(reader.GetOrdinal("OwnerName"))

                                };

                                walker.Walks.Add(walk);

                            }
                        }

                    }



                    reader.Close();

                    return walker;
                    
                        
                    
                }
            }
        }


        //GET WALKERS BY NEIGHBORHOOD

        public List<Walker> GetWalkersInNeighborhood(int neighborhoodId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                SELECT Id, [Name], ImageUrl, NeighborhoodId
                FROM Walker
                WHERE NeighborhoodId = @neighborhoodId
            ";

                    cmd.Parameters.AddWithValue("@neighborhoodId", neighborhoodId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Walker> walkers = new List<Walker>();
                    while (reader.Read())
                    {
                        Walker walker = new Walker
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl")),
                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId"))
                        };

                        walkers.Add(walker);
                    }

                    reader.Close();

                    return walkers;
                }
            }
        }


    }
}





//if (!reader.IsDBNull(reader.GetOrdinal("OwnerId")))
//{
//    walker.Clients = new List<Owner>();
//    Owner owner = new Owner
//    {
//        Id = reader.GetInt32(reader.GetOrdinal("OwnerId")),
//        Name = reader.GetString(reader.GetOrdinal("OwnerName")),

//    };

//walker.Clients.Add(owner);

//}