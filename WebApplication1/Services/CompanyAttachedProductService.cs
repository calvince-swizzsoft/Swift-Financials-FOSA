using Application.MainBoundedContext.DTO.AdministrationModule;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace WebApplication1.Services
{
    public class CompanyAttachedProductService
    {
        private readonly string _connectionString;
        private readonly SavingsProductService _savingsProductService;

        public CompanyAttachedProductService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["SwiftFin_Dev"].ConnectionString;
            _savingsProductService = new SavingsProductService();
        }

        public IEnumerable<CompanyAttachedProductDTO> GetAll()
        {
            var list = new List<CompanyAttachedProductDTO>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT cap.*, 
                                c.Description as CompanyDescription,
                                sp.Description as ProductDescription,
                                sp.Code as ProductCodeValue
                                FROM [swiftFin_CompanyAttachedProducts] cap
                                LEFT JOIN [swiftFin_Companies] c ON cap.CompanyId = c.Id
                                LEFT JOIN [swiftFin_SavingsProducts] sp ON cap.TargetProductId = sp.Id
                                ORDER BY c.Description, sp.Code";
                using (var cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            list.Add(Map(reader));
                }
            }
            return list;
        }

        public CompanyAttachedProductDTO GetById(Guid id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT cap.*, 
                                c.Description as CompanyDescription,
                                sp.Description as ProductDescription,
                                sp.Code as ProductCodeValue
                                FROM [swiftFin_CompanyAttachedProducts] cap
                                LEFT JOIN [swiftFin_Companies] c ON cap.CompanyId = c.Id
                                LEFT JOIN [swiftFin_SavingsProducts] sp ON cap.TargetProductId = sp.Id
                                WHERE cap.Id = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            return Map(reader);
                    }
                }
            }
            return null;
        }

        public IEnumerable<CompanyAttachedProductDTO> GetByCompanyId(Guid companyId)
        {
            var list = new List<CompanyAttachedProductDTO>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT cap.*, 
                                c.Description as CompanyDescription,
                                sp.Description as ProductDescription,
                                sp.Code as ProductCodeValue
                                FROM [swiftFin_CompanyAttachedProducts] cap
                                LEFT JOIN [swiftFin_Companies] c ON cap.CompanyId = c.Id
                                LEFT JOIN [swiftFin_SavingsProducts] sp ON cap.TargetProductId = sp.Id
                                WHERE cap.CompanyId = @CompanyId
                                ORDER BY sp.Code";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CompanyId", companyId);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            list.Add(Map(reader));
                }
            }
            return list;
        }

        public IEnumerable<CompanyAttachedProductDTO> GetByProductId(Guid productId)
        {
            var list = new List<CompanyAttachedProductDTO>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT cap.*, 
                                c.Description as CompanyDescription,
                                sp.Description as ProductDescription,
                                sp.Code as ProductCodeValue
                                FROM [swiftFin_CompanyAttachedProducts] cap
                                LEFT JOIN [swiftFin_Companies] c ON cap.CompanyId = c.Id
                                LEFT JOIN [swiftFin_SavingsProducts] sp ON cap.TargetProductId = sp.Id
                                WHERE cap.TargetProductId = @ProductId
                                ORDER BY c.Description";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductId", productId);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            list.Add(Map(reader));
                }
            }
            return list;
        }

        public CompanyAttachedProductDTO GetByCompanyAndProduct(Guid companyId, Guid productId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT cap.*, 
                                c.Description as CompanyDescription,
                                sp.Description as ProductDescription,
                                sp.Code as ProductCodeValue
                                FROM [swiftFin_CompanyAttachedProducts] cap
                                LEFT JOIN [swiftFin_Companies] c ON cap.CompanyId = c.Id
                                LEFT JOIN [swiftFin_SavingsProducts] sp ON cap.TargetProductId = sp.Id
                                WHERE cap.CompanyId = @CompanyId AND cap.TargetProductId = @ProductId";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CompanyId", companyId);
                    cmd.Parameters.AddWithValue("@ProductId", productId);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            return Map(reader);
                    }
                }
            }
            return null;
        }

        public CompanyAttachedProductDTO Create(CompanyAttachedProductDTO attachedProduct)
        {
            // Validate required fields
            if (attachedProduct.CompanyId == Guid.Empty)
                throw new ArgumentException("Company ID is required");

            if (attachedProduct.TargetProductId == Guid.Empty)
                throw new ArgumentException("Target Product ID is required");

            // Check if the product exists
            var product = _savingsProductService.GetById(attachedProduct.TargetProductId);
            if (product == null)
                throw new ArgumentException("Target product does not exist");

            // Check if this company already has this product attached
            var existing = GetByCompanyAndProduct(attachedProduct.CompanyId, attachedProduct.TargetProductId);
            if (existing != null)
                throw new InvalidOperationException("This product is already attached to this company");

            using (var conn = new SqlConnection(_connectionString))
            {
                // Generate new Guid if not provided
                if (attachedProduct.Id == Guid.Empty)
                    attachedProduct.Id = Guid.NewGuid();

                attachedProduct.CreatedDate = DateTime.Now;

                string query = @"INSERT INTO [swiftFin_CompanyAttachedProducts] 
                                ([Id], [CompanyId], [ProductCode], [TargetProductId], [CreatedDate])
                                VALUES 
                                (@Id, @CompanyId, @ProductCode, @TargetProductId, @CreatedDate)";

                using (var cmd = new SqlCommand(query, conn))
                {
                    AddParams(cmd, attachedProduct);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            return GetById(attachedProduct.Id);
        }

        public void CreateMultipleForCompany(Guid companyId, List<Guid> productIds)
        {
            if (productIds == null || productIds.Count == 0)
                throw new ArgumentException("At least one product ID is required");

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var productId in productIds)
                        {
                            // Check if product exists
                            var product = _savingsProductService.GetById(productId);
                            if (product == null)
                                throw new ArgumentException($"Product with ID {productId} does not exist");

                            // Check if already attached
                            var existing = GetByCompanyAndProduct(companyId, productId);
                            if (existing == null)
                            {
                                var attachedProduct = new CompanyAttachedProductDTO
                                {
                                    Id = Guid.NewGuid(),
                                    CompanyId = companyId,
                                    TargetProductId = productId,
                                    ProductCode = 1, // Assuming 1 for Savings product
                                    CreatedDate = DateTime.Now
                                };

                                string query = @"INSERT INTO [swiftFin_CompanyAttachedProducts] 
                                                ([Id], [CompanyId], [ProductCode], [TargetProductId], [CreatedDate])
                                                VALUES 
                                                (@Id, @CompanyId, @ProductCode, @TargetProductId, @CreatedDate)";

                                using (var cmd = new SqlCommand(query, conn, transaction))
                                {
                                    AddParams(cmd, attachedProduct);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void AttachMandatoryProductsToCompany(Guid companyId)
        {
            // Get all mandatory savings products
            var mandatoryProducts = _savingsProductService.GetMandatoryProducts();

            var productIds = new List<Guid>();
            foreach (var product in mandatoryProducts)
            {
                productIds.Add(product.Id);
            }

            if (productIds.Count > 0)
            {
                CreateMultipleForCompany(companyId, productIds);
            }
        }

        public void Delete(Guid id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM [swiftFin_CompanyAttachedProducts] WHERE Id = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteByCompanyAndProduct(Guid companyId, Guid productId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM [swiftFin_CompanyAttachedProducts] WHERE CompanyId = @CompanyId AND TargetProductId = @ProductId";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CompanyId", companyId);
                    cmd.Parameters.AddWithValue("@ProductId", productId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteAllByCompany(Guid companyId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM [swiftFin_CompanyAttachedProducts] WHERE CompanyId = @CompanyId";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CompanyId", companyId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void AddParams(SqlCommand cmd, CompanyAttachedProductDTO attachedProduct)
        {
            cmd.Parameters.AddWithValue("@Id", attachedProduct.Id);
            cmd.Parameters.AddWithValue("@CompanyId", attachedProduct.CompanyId);
            cmd.Parameters.AddWithValue("@ProductCode", attachedProduct.ProductCode);
            cmd.Parameters.AddWithValue("@TargetProductId", attachedProduct.TargetProductId);
            cmd.Parameters.AddWithValue("@CreatedDate", attachedProduct.CreatedDate);
        }

        private CompanyAttachedProductDTO Map(IDataReader reader)
        {
            return new CompanyAttachedProductDTO
            {
                Id = (Guid)reader["Id"],
                CompanyId = (Guid)reader["CompanyId"],
                ProductCode = Convert.ToInt32(reader["ProductCode"]),
                TargetProductId = (Guid)reader["TargetProductId"],
                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                Company = new CompanyDTO
                {
                    Id = (Guid)reader["CompanyId"],
                    Description = reader["CompanyDescription"]?.ToString()
                }
            };
        }
    }
}