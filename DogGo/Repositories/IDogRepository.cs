using DogGo.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public interface IDogRepository
    {
      /*  List<Owner> GetAllDogs(); */
        List<Dog> GetDogByOwnerId(int ownerId);
    }
}