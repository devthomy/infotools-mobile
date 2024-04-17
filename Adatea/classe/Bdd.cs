using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Adatea.classe
{
    public class Bdd
    {
        public async Task<List<Appointment>> LoadRendezVousAsync(int commercialId)
        {
            List<Appointment> appointments = new List<Appointment>();
            string connectionString = "server=localhost;database=adatea;user=root;password=root";
            using (var conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new MySqlCommand("SELECT * FROM appointment WHERE ID_Commercial = @CommercialId", conn))
                {
                    cmd.Parameters.AddWithValue("@CommercialId", commercialId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var appointment = new Appointment
                            {
                                ID_Rdv = reader.GetInt32(reader.GetOrdinal("ID_Rdv")),
                                ID_Client = reader.IsDBNull(reader.GetOrdinal("ID_Client")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("ID_Client")),
                                ID_Commercial = reader.GetInt32(reader.GetOrdinal("ID_Commercial")),
                                Date_Rdv = reader.GetDateTime(reader.GetOrdinal("Date_Rdv")),
                                Location = reader.GetString(reader.GetOrdinal("Location"))
                            };
                            appointments.Add(appointment);
                        }
                    }
                }
            }
            return appointments;
        }

        public async Task<bool> DeleteAppointmentAsync(int idRdv)
        {
            string connectionString = "server=localhost;database=adatea;user=root;password=root";
            using (var conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new MySqlCommand("DELETE FROM appointment WHERE ID_Rdv = @IDRdv", conn))
                {
                    cmd.Parameters.AddWithValue("@IDRdv", idRdv);
                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }



        public async Task<Commercial> GetCommercialByEmailAndPasswordAsync(string email, string password)
        {
            string connectionString = "server=localhost;database=adatea;user=root;password=root";
            using (var conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new MySqlCommand("SELECT * FROM commercial WHERE Email = @Email AND Password = @Password", conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var commercial = new Commercial
                            {
                                ID_Commercial = reader.GetInt32(reader.GetOrdinal("ID_Commercial")),
                                Lastname = reader.GetString(reader.GetOrdinal("Lastname")),
                                Firstname = reader.GetString(reader.GetOrdinal("Firstname")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                            };
                            return commercial;
                        }
                    }
                }
            }
            return null;
        }




        public async Task<bool> AddAppointmentAsync(Appointment appointment)
        {
            string connectionString = "server=localhost;database=adatea;user=root;password=root";
            using (var conn = new MySqlConnection(connectionString))
            {
                try
                {
                    await conn.OpenAsync();
                    string query = @"INSERT INTO appointment (ID_Client, ID_Commercial, Date_Rdv, Time_Rdv, Location) 
                                     VALUES (@ID_Client, @ID_Commercial, @Date_Rdv, @Time_Rdv, @Location)";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID_Client", appointment.ID_Client);
                        cmd.Parameters.AddWithValue("@ID_Commercial", appointment.ID_Commercial);
                        cmd.Parameters.AddWithValue("@Date_Rdv", appointment.Date_Rdv);
                        cmd.Parameters.AddWithValue("@Time_Rdv", appointment.Time_Rdv);
                        cmd.Parameters.AddWithValue("@Location", appointment.Location);

                        var result = await cmd.ExecuteNonQueryAsync();
                        return result > 0; // Retourne true si l'insertion est réussie
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Une erreur est survenue lors de l'ajout du rendez-vous: {ex.Message}");
                    return false;
                }
            }
        }




        public async Task<bool> UpdateAppointmentAsync(Appointment appointment)
        {
            string connectionString = "server=localhost;database=adatea;user=root;password=root";
            using (var conn = new MySqlConnection(connectionString))
            {
                try
                {
                    await conn.OpenAsync();
                    string query = @"UPDATE appointment SET 
                             ID_Client = @ID_Client, 
                             Date_Rdv = @Date_Rdv, 
                             Time_Rdv = @Time_Rdv, 
                             Location = @Location 
                             WHERE ID_Rdv = @ID_Rdv";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID_Rdv", appointment.ID_Rdv);
                        cmd.Parameters.AddWithValue("@ID_Client", appointment.ID_Client ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Date_Rdv", appointment.Date_Rdv);
                        cmd.Parameters.AddWithValue("@Time_Rdv", appointment.Time_Rdv);
                        cmd.Parameters.AddWithValue("@Location", appointment.Location);

                        var result = await cmd.ExecuteNonQueryAsync();
                        return result > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Une erreur est survenue lors de la mise à jour du rendez-vous: {ex.Message}");
                    return false;
                }
            }
        }

    }
}
