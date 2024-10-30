$(document).ready(function () {
	const watermarkContainer = document.querySelector('.watermark-container');
	const watermarkText = 'AGGO COLLEGE MAKURDI';
	const watermarkFontSize = 30; // Adjust font size as needed
	const watermarkMargin = 50;

	function createWatermarks() {
		const containerWidth = watermarkContainer.clientWidth;
		const containerHeight = watermarkContainer.clientHeight;
		const textWidth = watermarkFontSize * watermarkText.length;
		const textHeight = watermarkFontSize;

		let x = watermarkMargin;
		let y = watermarkMargin;

		// Ensure watermarks fill the entire container space
		while (y < containerHeight) {
			while (x < containerWidth) {
				const watermark = document.createElement('div');
				watermark.textContent = watermarkText;
				watermark.classList.add('watermark');
				watermark.style.left = x + 'px';
				watermark.style.top = y + 'px';
				watermarkContainer.appendChild(watermark);

				x += textWidth + watermarkMargin;
			}
			x = watermarkMargin; // Reset x for next row
			y += textHeight + watermarkMargin;
		}
	}

	createWatermarks();
});