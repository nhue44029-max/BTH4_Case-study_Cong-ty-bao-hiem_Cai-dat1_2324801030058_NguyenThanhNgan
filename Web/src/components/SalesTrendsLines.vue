<template>
  <div class="card">
    <div class="card-header">Trends
      <div class="card-header-actions">
        <a class="card-header-action" href="http://www.chartjs.org" target="_blank">
          <small class="text-muted">docs</small>
        </a>
      </div>
    </div>
    <div class="card-body">
      <div class="chart-wrapper">
        <SalesLines :chart-data="datacollection"/>
      </div>
    </div>
  </div>
</template>

<script>
import SalesLines from "./SalesLines.js";

export default {
  name: "SalesTrendsLines",
  components: { SalesLines },
  props: {
    salesTrendsData: { type: Array, required: true }
  },
  computed: {
    datacollection() {
      return {
        labels: this.salesTrendsData.map(point => point.period),
        datasets: [{
          data: this.salesTrendsData.map(point => point.amount),
          label: "Sales (EUR)",
          borderColor: "#f34f76",
          backgroundColor: "rgba(255, 84, 126, 0.76)",
          pointBackgroundColor: "#f34f76",
          cubicInterpolationMode: "monotone",
          pointRadius: 3,
          fill: "origin"
        }]
      };
    }
  }
};
</script>


<style>
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
