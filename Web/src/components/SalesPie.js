import { Pie, mixins } from 'vue-chartjs'
const { reactiveProp } = mixins

export default {
  extends: Pie,
  mixins: [reactiveProp],
  data() {
    return {
      options: {
        responsive: true,
        maintainAspectRatio: false,
        legend: { display: false },
        onClick: (event, elements) => {
          if (elements.length) this.$emit("select", elements[0]._index);
        }
      }
    };
  },

  mounted () {
    this.renderChart(this.chartData, this.options)
  }
}
