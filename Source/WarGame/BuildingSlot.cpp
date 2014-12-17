// Fill out your copyright notice in the Description page of Project Settings.

#include "WarGame.h"

#include "BuildingSlot.h"

#include "RTS_GameMode.h"
#include "RTS_Camera.h"

ABuildingSlot::ABuildingSlot(const FObjectInitializer& ObjectInitializer)
	: Super(ObjectInitializer)
{

	PrimaryActorTick.bStartWithTickEnabled = true;
	PrimaryActorTick.bCanEverTick = true;

	building = nullptr;

}

void ABuildingSlot::init(UWidgetComponent *gui)
{
	widget = gui;

	// Makes no difference? Works on Tick().
	widget->SetRelativeLocation(FVector(0, 0, 1800));

	GEngine->AddOnScreenDebugMessage(-1, 15.0f, FColor::Red, FString::Printf(TEXT("GUI: %p"), widget));

	HideOptions();
}


void ABuildingSlot::Tick(float DeltaSeconds)
{
	Super::Tick(DeltaSeconds);

	if (widget && widget->IsVisible()){

		APlayerController *controller = GetWorld()->GetFirstPlayerController();
		if (!controller)
			return;

		ARTS_Camera *rtsCamera = Cast<ARTS_Camera>(controller->GetPawn());
		if (!rtsCamera)
			return;

		// Resize
		float dist = FVector::Dist(rtsCamera->camera->GetComponentLocation(), GetActorLocation()) / 1100.0;
		widget->SetWorldScale3D( FVector(dist, dist, dist) );
		// GEngine->AddOnScreenDebugMessage(-1, 15.0f, FColor::Red, FString::Printf(TEXT("Scale: %f"), dist));

		// Look direction
		widget->SetRelativeRotation( FRotator(0, 90, 0) );

		// This should be on Init() call
		widget->SetRelativeLocation(FVector(0, 0, 1800));
	}
}

bool ABuildingSlot::OnBuildOnSlot(EBuildingTypes type)
{
	GEngine->AddOnScreenDebugMessage(-1, 15.0f, FColor::Red, FString::Printf(TEXT("OnBuildOnSlot: %d"), (int)type));

	if (building){
		return false;
	}

	ARTS_GameMode* gm = Cast<ARTS_GameMode>(GetWorld()->GetAuthGameMode());

	// GEngine->AddOnScreenDebugMessage(-1, 15.0f, FColor::Red, FString::Printf(TEXT("GM: %p"), gm));

	if (!gm)
		return false;

	// Create the building object
	switch (type){

		case EBuildingTypes::BT_Ranger:
			building = GetWorld()->SpawnActor<ABuilding>(gm->buildingRanger);
			break;

		case EBuildingTypes::BT_Barracks: 
			building = GetWorld()->SpawnActor<ABuilding>(gm->buildingBarracks);
			break;

		case EBuildingTypes::BT_Tower:
			building = GetWorld()->SpawnActor<ABuilding>(gm->buildingTower);
			break;

		case EBuildingTypes::BT_Gate:
			building = GetWorld()->SpawnActor<ABuilding>(gm->buildingGate);
			break;

		case EBuildingTypes::BT_Keep:
			building = GetWorld()->SpawnActor<ABuilding>(gm->buildingKeep);
			break;

		case EBuildingTypes::BT_GoldMine:
			building = GetWorld()->SpawnActor<ABuilding>(gm->buildingGoldMine);
			break;

		case EBuildingTypes::BT_StoneMine:
			building = GetWorld()->SpawnActor<ABuilding>(gm->buildingStoneMine);
			break;

		default:
			return false;

	}

	building->SetActorLocation(GetActorLocation());
	HideOptions();

	return true;
}

void ABuildingSlot::ShowOptions()
{
	if (building){
		building->ShowOptions();
	}
	else if (widget)
	{
		widget->SetHiddenInGame(false);
		widget->SetVisibility(true);
		widget->Activate();
	}

	// GEngine->AddOnScreenDebugMessage(-1, 15.0f, FColor::Red, FString::Printf(TEXT("SHOW: %p"), widget));
}

void ABuildingSlot::HideOptions()
{
	if (building){
		building->HideOptions();
	}
	
	if (widget){
		widget->SetHiddenInGame(true);
		widget->SetVisibility(false);
		widget->Deactivate();
	}

	// GEngine->AddOnScreenDebugMessage(-1, 15.0f, FColor::Red, FString::Printf(TEXT("HIDE: %p"), widget));
}