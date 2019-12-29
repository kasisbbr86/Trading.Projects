// Set new default font family and font color to mimic Bootstrap's default styling
Chart.defaults.global.defaultFontFamily = 'Nunito', '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
Chart.defaults.global.defaultFontColor = '#858796';

// Pie Chart Example
var ctx = document.getElementById("myPieChart");
var myPieChart = new Chart(ctx, {
  type: 'doughnut',
  data: {
    labels: ["Delivered", "Out For Delivery", "In Transit", "Failed Attempts", "Info received"],
    datasets: [{
      data: [283, 383, 283 , 223 , 245],
      backgroundColor: ['#4e73df', '#1cc88a', '#36b9cc','#e74a3b','#f6c23e'],
      hoverBackgroundColor: ['#2e59d9', '#17a673', '#2c9faf','#cc594e','#e6b949'],
      hoverBorderColor: "rgba(234, 236, 244, 1)",
    }],
  },
  options: {
    maintainAspectRatio: false,
    tooltips: {
      backgroundColor: "rgb(255,255,255)",
      bodyFontColor: "#858796",
      borderColor: '#dddfeb',
      borderWidth: 1,
      xPadding: 15,
      yPadding: 15,
      displayColors: false,
      caretPadding: 10,
    },
	
    legend: {
      display: false
    },
    cutoutPercentage: 80,
  },
});


// Pie Chart Example




// Pie Chart Example
var ctx = document.getElementById("myPieChart1");
var myPieChart = new Chart(ctx, {
  type: 'pie',
  data: {
    labels: ["Germany", "China", "France", "UAE" ,"USA"],
    datasets: [{
      data: [28,20,25,30,32],
      backgroundColor: ['#4e73df', '#1cc88a', '#36b9cc','#e74a3b','#f6c23e'],
      hoverBackgroundColor: ['#2e59d9', '#17a673', '#2c9faf','#cc594e','#e6b949'],
      hoverBorderColor: "rgba(234, 236, 244, 1)",
    }],
  },
  options: {
    maintainAspectRatio: false,
    tooltips: {
      backgroundColor: "rgb(255,255,255)",
      bodyFontColor: "#858796",
      borderColor: '#dddfeb',
      borderWidth: 1,
      xPadding: 15,
      yPadding: 15,
      displayColors: false,
      caretPadding: 10,
    },
	
    legend: {
      display: false
    },
    
  },
});


// Pie Chart Example
var ctx = document.getElementById("myPieChart2");
var myPieChart = new Chart(ctx, {
  type: 'pie',
  data: {
    labels: ["Courier", "Ground Service", "NextDay Air", "SecondDay Air " ,"TrucksOld"],
    datasets: [{
      data: [10,40,15,50,68],
      backgroundColor: ['#4e73df', '#1cc88a', '#36b9cc','#e74a3b','#f6c23e'],
      hoverBackgroundColor: ['#2e59d9', '#17a673', '#2c9faf','#cc594e','#e6b949'],
      hoverBorderColor: "rgba(234, 236, 244, 1)",
    }],
  },
  options: {
    maintainAspectRatio: false,
    tooltips: {
      backgroundColor: "rgb(255,255,255)",
      bodyFontColor: "#858796",
      borderColor: '#dddfeb',
      borderWidth: 1,
      xPadding: 15,
      yPadding: 15,
      displayColors: false,
      caretPadding: 15,
    },
	
    legend: {
      display: false
    },
    cutoutPercentage: 10,
  },
});
