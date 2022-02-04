using DogGo.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public interface INeighborhoodRepository
    {
        List<Neighborhood> GetAll();
       

    }
}