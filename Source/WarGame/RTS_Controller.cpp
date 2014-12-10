// Fill out your copyright notice in the Description page of Project Settings.

#include "WarGame.h"
#include "RTS_Controller.h"

void ARTS_Controller::BeginPlay()
{
	Super::BeginPlay();

	bShowMouseCursor = true;
	bEnableClickEvents = true;
	bEnableMouseOverEvents = true;

	// ViewTarget = GetWorld()->SpawnActor<ARTS_Camera>();

	// Alterar view target
	// SetViewTarget(ViewTarget->camera);
}



void ARTS_Controller::setCurrentSelectedSlot(UObject *slot)
{
	// Hide options for that object
	if (currentBuildingSlot != nullptr){
		//currentBuildingSlot->HideOptions();
	}

	currentBuildingSlot = slot;

	// Show options for selected object
	if (currentBuildingSlot != nullptr){
		//currentBuildingSlot->ShowOptions();
	}
	
}