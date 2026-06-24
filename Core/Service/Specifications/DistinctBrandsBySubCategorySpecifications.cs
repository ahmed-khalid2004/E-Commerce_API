using DomainLayer.Models.ProductModule;

namespace Service.Specifications
{
    /// <summary>
    /// Returns ProductBrand rows that have at least one Product under the
    /// given SubCategory. No Brand<->SubCategory FK exists by design — this
    /// is the "infer from Products" approach instead of adding a relation.
    /// </summary>
    class DistinctBrandsBySubCategorySpecifications : BaseSpecifications<ProductBrand, int>
    {
        public DistinctBrandsBySubCategorySpecifications(int subCategoryId)
            : base(b => b.Products.Any(p => p.SubCategoryId == subCategoryId))
        {
            AddOrderBy(b => b.Name);
        }
    }
}