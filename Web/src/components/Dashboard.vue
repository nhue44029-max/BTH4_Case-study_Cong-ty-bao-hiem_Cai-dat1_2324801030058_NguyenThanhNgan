<template>
  <div>
    <div class="container-fluid">
      <div class="row">
        <div class="col text-left">
          <div>
            <b-dropdown id="dropdown-1" :text="selectedProductName" class="m-md-2">
              <b-dropdown-item v-for="option in productsFilterOptions" 
              :active="option.value === selectedProduct"
              :key="option.value"
              @click="selectedProduct = option.value">
              {{option.text}}
              </b-dropdown-item>
            </b-dropdown>
          </div>
        </div>
        <div class="col">
          <div class="float-right">
          <b-button-group aria-label="Sales period">
            <b-button v-for="period in periods" :key="period.value"
                      :variant="selectedPeriod === period.value ? 'info' : 'outline-info'"
                      :pressed="selectedPeriod === period.value"
                      @click="selectedPeriod = period.value">
              {{ period.text }}
            </b-button>
          </b-button-group>
          </div>
        </div>
        

      </div>
    </div>

    <div class="container-fluid my-3" aria-live="polite">
      <p class="mb-0">{{ selectedProductName }} · {{ dateRange.start.format('DD/MM/YYYY') }} – {{ dateRange.end.format('DD/MM/YYYY') }}</p>
      <p v-if="loading" class="mb-0 text-muted">Loading sales data…</p>
      <p v-else-if="error" class="mb-0 text-danger" role="alert">{{ error }}</p>
    </div>

    <div v-if="!loading && !error" class="container-fluid">
      <div class="row">
        <TotalSalesCard title="Total sales" :amount="totalAmount" icon="bitcoin" />

        <TotalSalesCard title="Num. policies" :amount="totalPolicies" icon="book" />

        <TotalSalesCard
          v-for="product in products"
          :key="product.code"
          :title="product.name"
          :amount="productSales(product.code).premiumAmount"
          :policiesCount="productSales(product.code).policiesCount"
          :icon="product.icon"
        />
      </div>
    </div>

    <div class="container-fluid">
      <div class="row">
        <div>&nbsp;</div>
      </div>
    </div>

    <div v-if="!loading && !error" class="container-fluid">
      <div class="card-columns cols-2">
        <SalesTrendsLines :salesTrendsData="salesTrends" />
        <SalesDistribution :products="products" :product-totals="totals.perProductTotal"
                           :selected-product="selectedProduct" @select="selectedProduct = $event" />
        <SalesAgents :agentTotals="agentsTotalAmounts" />
      </div>
    </div>
  </div>
</template>

<script>
import { HTTP } from "./http/ApiClient";
import SalesAgents from "./SalesAgents";
import SalesTrendsLines from "./SalesTrendsLines";
import SalesDistribution from "./SalesDistribution";
import TotalSalesCard from "./TotalSalesCard";
import moment from "moment";

const noSales = { premiumAmount: 0, policiesCount: 0 };

export default {
  name: "Dashboard",
  components: { SalesAgents, SalesTrendsLines, SalesDistribution, TotalSalesCard },
  data() {
    return {
      products: [],
      totals: { total: noSales, perProductTotal: {} },
      agentsTotalAmounts: {},
      periodsSales: [],
      selectedProduct: "ALL",
      selectedPeriod: "LAST_12_MONTHS",
      periods: [
        { text: "Last 12 Months", value: "LAST_12_MONTHS" },
        { text: "This year", value: "THIS_YEAR" },
        { text: "This month", value: "THIS_MONTH" }
      ],
      loading: true,
      error: "",
      requestId: 0
    };
  },
  computed: {
    productsFilterOptions() {
      return [
        { text: "All products", value: "ALL" },
        ...this.products.map(product => ({ text: product.name, value: product.code }))
      ];
    },
    selectedProductName() {
      return this.productsFilterOptions.find(option => option.value === this.selectedProduct).text;
    },
    dateRange() {
      const end = moment();
      if (this.selectedPeriod === "THIS_MONTH")
        return { start: end.clone().startOf("month"), end, bucket: "day" };
      const start = this.selectedPeriod === "THIS_YEAR"
        ? end.clone().startOf("year")
        : end.clone().startOf("month").subtract(11, "months");
      return { start, end, bucket: "month" };
    },
    selectedSales() {
      return this.selectedProduct === "ALL" ? this.totals.total : this.productSales(this.selectedProduct);
    },
    totalAmount() {
      return this.selectedSales.premiumAmount;
    },
    totalPolicies() {
      return this.selectedSales.policiesCount;
    },
    salesTrends() {
      const { start, end, bucket } = this.dateRange;
      const key = bucket === "day" ? "YYYY-MM-DD" : "YYYY-MM";
      const label = bucket === "day" ? "DD/MM" : "MM/YYYY";
      const byDate = new Map(this.periodsSales.map(period => [
        moment.utc(period.periodDate).format(key), period.sales.premiumAmount
      ]));
      const series = [];
      for (const date = start.clone(); date.isSameOrBefore(end, bucket); date.add(1, bucket)) {
        series.push({ period: date.format(label), amount: byDate.get(date.format(key)) || 0 });
      }
      return series;
    }
  },
  watch: {
    selectedProduct: "refresh",
    selectedPeriod: "refresh"
  },
  async created() {
    try {
      this.products = (await HTTP.get("products")).data;
      await this.refresh();
    } catch (error) {
      this.loading = false;
      this.error = "Could not load the product list.";
    }
  },
  methods: {
    productSales(code) {
      return this.totals.perProductTotal[code] || noSales;
    },
    async refresh() {
      const id = ++this.requestId;
      const { start, end, bucket } = this.dateRange;
      const dates = { salesDateFrom: start.format("YYYY-MM-DD"), salesDateTo: end.format("YYYY-MM-DD") };
      const filters = { ...dates, productCode: this.selectedProduct === "ALL" ? null : this.selectedProduct };
      this.loading = true;
      this.error = "";
      try {
        const [totals, agents, trends] = await Promise.all([
          HTTP.post("dashboard/total-sales", dates),
          HTTP.post("dashboard/agents-sales", filters),
          HTTP.post("dashboard/sales-trends", { ...filters, unit: bucket === "day" ? "Day" : "Month" })
        ]);
        if (id !== this.requestId) return;
        this.totals = totals.data;
        const amounts = {};
        for (const [agent, sales] of Object.entries(agents.data.perAgentTotal)) {
          amounts[agent] = sales.premiumAmount;
        }
        this.agentsTotalAmounts = amounts;
        this.periodsSales = trends.data.periodsSales;
      } catch (error) {
        if (id === this.requestId) this.error = "Could not load sales data. Reload the dashboard to try again.";
      } finally {
        if (id === this.requestId) this.loading = false;
      }
    }
  }
};
</script>

<style scoped>
h3 {
  margin: 40px 0 0;
}
ul {
  list-style-type: none;
  padding: 0;
}
li {
  display: inline-block;
  margin: 0 10px;
}
a {
  color: #42b983;
}
.arch-image {
  width: 100%;
}
@media (min-width: 576px) {
  .card-columns.cols-2 {
    -webkit-column-count: 2;
    -moz-column-count: 2;
    column-count: 2;
  }
  .card-header-actions {
    display: inline-block;
    float: right;
    margin-right: -0.25rem;
  }
  *[dir="rtl"] .card-header-actions {
    float: left;
    margin-right: auto;
    margin-left: -0.25rem;
  }
  .card-header-action {
    padding: 0 0.25rem;
    color: #73818f;
  }
  .card-header-action:hover {
    color: #23282c;
    text-decoration: none;
  }
}
</style>