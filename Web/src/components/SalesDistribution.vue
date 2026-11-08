<template>
  <div class="card">
    <div class="card-header">Products</div>
    <div class="card-body">
      <p class="small text-muted">Sales by product for the selected period</p>
      <sales-pie v-if="hasSales" :chart-data="chartData" @select="selectSlice" />
      <p v-else class="text-muted">No product sales in this period.</p>
      <ul class="list-unstyled mb-0">
        <li v-for="(product, index) in products" :key="product.code" class="mb-1">
          <button type="button" class="btn btn-sm btn-block text-left"
                  :class="selectedProduct === product.code ? 'btn-light font-weight-bold' : 'btn-link'"
                  :aria-pressed="String(selectedProduct === product.code)"
                  @click="$emit('select', product.code)">
            <span class="product-color" :style="{ backgroundColor: colors[index % colors.length] }" aria-hidden="true"></span>
            {{ product.name }}
            <span class="float-right">{{ sales(product.code).premiumAmount }} EUR · {{ sales(product.code).policiesCount }} policies</span>
          </button>
        </li>
      </ul>
    </div>
  </div>
</template>

<script>
import SalesPie from "./SalesPie.js";

const noSales = { premiumAmount: 0, policiesCount: 0 };

export default {
  name: "SalesDistribution",
  components: { SalesPie },
  props: {
    products: { type: Array, required: true },
    productTotals: { type: Object, required: true },
    selectedProduct: { type: String, required: true }
  },
  data() {
    return { colors: ["#d84768", "#217db0", "#b87800", "#6574c4"] };
  },
  computed: {
    hasSales() {
      return this.products.some(product => this.sales(product.code).premiumAmount > 0);
    },
    chartData() {
      return {
        labels: this.products.map(product => product.name),
        datasets: [{
          data: this.products.map(product => this.sales(product.code).premiumAmount),
          backgroundColor: this.products.map((product, index) => {
            const color = this.colors[index % this.colors.length];
            return this.selectedProduct === "ALL" || this.selectedProduct === product.code ? color : color + "55";
          })
        }]
      };
    }
  },
  methods: {
    sales(code) {
      return this.productTotals[code] || noSales;
    },
    selectSlice(index) {
      this.$emit("select", this.products[index].code);
    }
  }
};
</script>

<style scoped>
.product-color {
  display: inline-block;
  width: 0.75rem;
  height: 0.75rem;
  margin-right: 0.3rem;
  border-radius: 2px;
}
</style>
